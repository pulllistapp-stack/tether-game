using UnityEngine;

namespace Tether.Gameplay
{
    /// <summary>
    /// Baseline Ball. Bounces off walls/enemies via Rigidbody2D physics.
    /// Phase 2: reads optional BallData for speed/damage/color/behavior; falls
    /// back to inspector defaults when no data is assigned.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Ball : MonoBehaviour
    {
        [Header("Fallback config (used when BallData is null)")]
        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private int _maxBounces = -1;
        [SerializeField] private bool _preserveSpeedOnBounce = true;

        [Header("Runtime refs")]
        [SerializeField] private BallData _data;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private TrailRenderer _trail;

        [Header("Enemy layer for AoE queries")]
        [SerializeField] private LayerMask _enemyLayers = ~0;

        [Header("Return to Player")]
        [Tooltip("After this many bounces the ball turns off collisions and homes back to the player, then disappears on arrival. -1 disables.")]
        [SerializeField] private int _returnAfterBounces = 5;
        [SerializeField] private float _returnSpeed = 14f;
        [SerializeField] private float _returnPickupRadius = 0.4f;

        private Rigidbody2D _rb;
        private Collider2D _collider;
        private int _bounceCount;
        private int _splitDepth;
        private bool _returning;
        private Transform _returnTarget;

        public BallData Data => _data;

        public void SetSplitDepth(int depth) => _splitDepth = depth;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.freezeRotation = true;
            if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
            if (_trail == null)  _trail  = GetComponent<TrailRenderer>();
        }

        private void FixedUpdate()
        {
            if (!_returning) return;

            if (_returnTarget == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) _returnTarget = p.transform;
                if (_returnTarget == null) { Destroy(gameObject); return; }
            }

            Vector2 pos = _rb.position;
            Vector2 toPlayer = ((Vector2)_returnTarget.position - pos);
            float dist = toPlayer.magnitude;
            if (dist <= _returnPickupRadius) { Destroy(gameObject); return; }

            Vector2 dir = toPlayer / Mathf.Max(0.001f, dist);
            _rb.linearVelocity = dir * _returnSpeed;
        }

        private void EnterReturnMode()
        {
            _returning = true;
            // Disable further bounce/hit interactions so the ball doesn't ricochet on the way home
            if (_collider != null) _collider.enabled = false;
            // Cool-down the trail alpha so the return arc reads differently
            if (_trail != null)
            {
                var g = _trail.colorGradient;
                var alphaKeys = new GradientAlphaKey[] {
                    new GradientAlphaKey(0.35f, 0f),
                    new GradientAlphaKey(0f, 1f)
                };
                var newG = new Gradient();
                newG.SetKeys(g.colorKeys, alphaKeys);
                _trail.colorGradient = newG;
            }
        }

        public void Configure(BallData data)
        {
            _data = data;
            if (data == null) return;

            if (_sprite != null) _sprite.color = data.tintColor;
            if (_trail  != null)
            {
                var grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(data.tintColor, 0f),
                        new GradientColorKey(data.tintColor * 0.7f, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(0.85f, 0f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
                _trail.colorGradient = grad;
            }
            float sizeMul = data.sizeMultiplier * SizeUpgradeMul();
            transform.localScale = Vector3.one * sizeMul;
        }

        public void Launch(Vector2 direction)
        {
            float speed = _data != null ? _data.speed : _speed;
            speed *= SpeedUpgradeMul();
            _rb.linearVelocity = direction.normalized * speed;
        }

        private float SpeedUpgradeMul() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.BallSpeedMul : 1f;
        private float DamageUpgradeMul() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.BallDamageMul : 1f;
        private float SizeUpgradeMul() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.BallSizeMul : 1f;
        private float ExplosionRadiusUpgradeMul() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.ExplosionRadiusMul : 1f;
        private int SplitCountUpgradeAdd() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.SplitCountAdd : 0;
        private int SplitDepthUpgradeAdd() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.SplitDepthAdd : 0;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _bounceCount++;

            bool hitEnemy = collision.gameObject.TryGetComponent<Enemy.Enemy>(out var enemy);
            bool hitWall  = collision.gameObject.GetComponent<Wall>() != null;

            float damage = (_data != null ? _data.damage : _damage) * DamageUpgradeMul();
            if (hitEnemy) enemy.TakeDamage(damage);

            // Subtle screen shake on wall bounce for kinetic feel
            if (hitWall && CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(0.06f);

            if (hitWall)  Audio.AudioManager.Instance?.Play("ball_bounce");
            if (hitEnemy) Audio.AudioManager.Instance?.Play("ball_hit_enemy");

            // Behavior branches
            var behavior = _data != null ? _data.behavior : BallBehavior.Normal;

            if (behavior == BallBehavior.Explosive && hitEnemy && _data != null)
            {
                float radius = _data.explosionRadius * ExplosionRadiusUpgradeMul();
                var hits = Physics2D.OverlapCircleAll(
                    collision.GetContact(0).point, radius, _enemyLayers);
                foreach (var h in hits)
                {
                    if (h.gameObject == collision.gameObject) continue;
                    if (h.TryGetComponent<Enemy.Enemy>(out var otherEnemy))
                        otherEnemy.TakeDamage(_data.explosionDamage * DamageUpgradeMul());
                }
            }

            int splitDepthLimit = (_data != null ? _data.splitMaxDepth : 0) + SplitDepthUpgradeAdd();
            if (behavior == BallBehavior.Split && hitWall && _data != null && _splitDepth < splitDepthLimit)
            {
                SpawnSplitChildren();
                Destroy(gameObject);
                return;
            }

            float speedNow = (_data != null ? _data.speed : _speed) * SpeedUpgradeMul();
            if (_preserveSpeedOnBounce)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * speedNow;
            }

            int maxB = _data != null ? _data.maxBounces : _maxBounces;
            if (maxB > 0 && _bounceCount >= maxB)
            {
                Destroy(gameObject);
                return;
            }

            // Return to player after N bounces (skips for Split children mid-tree to keep them lively)
            if (_returnAfterBounces > 0 && _bounceCount >= _returnAfterBounces && !_returning)
            {
                EnterReturnMode();
            }
        }

        private void SpawnSplitChildren()
        {
            if (_data == null) return;
            var childData = _data.splitChild != null ? _data.splitChild : _data;
            Vector2 baseDir = _rb.linearVelocity.normalized;
            if (baseDir == Vector2.zero) baseDir = Vector2.up;

            int count = _data.splitCount + SplitCountUpgradeAdd();
            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0f : (float)i / (count - 1);
                float angle = Mathf.Lerp(-_data.splitAngle, _data.splitAngle, t);
                Vector2 dir = Quaternion.Euler(0f, 0f, angle) * baseDir;

                var childObj = Instantiate(gameObject, transform.position, Quaternion.identity);
                if (childObj.TryGetComponent<Ball>(out var childBall))
                {
                    childBall.SetSplitDepth(_splitDepth + 1);
                    childBall.Configure(childData);
                    childBall.Launch(dir);
                }
            }
        }
    }
}
