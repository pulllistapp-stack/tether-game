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

        [Header("Lighting")]
        [SerializeField] private bool _emitLight = true;
        [SerializeField] private float _lightRadius = 2.4f;
        [SerializeField] private float _lightIntensity = 1.15f;

        [Header("Out of Bounds")]
        [Tooltip("Ball is destroyed once it falls behind the player past this Y (lets missed shots exit instead of bouncing forever).")]
        [SerializeField] private float _voidY = -9f;

        private Rigidbody2D _rb;
        private Collider2D _collider;
        private int _bounceCount;
        private int _splitDepth;
        private bool _returning;
        private Transform _returnTarget;
        private Vector2 _prevVelocity;

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

            // Balls are the brightest thing on screen — give each one a light so it
            // actually casts into the arena instead of just being a bright sprite.
            if (_emitLight && GetComponent<Utility.AutoLight2D>() == null)
            {
                var al = gameObject.AddComponent<Utility.AutoLight2D>();
                al.Configure(_lightRadius, _lightIntensity);
            }

            // Balls fly through each other — physical ball-vs-ball bounces made aimed
            // corner shots unpredictable, so this is disabled at the layer level.
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
        }

        private void FixedUpdate()
        {
            _prevVelocity = _rb.linearVelocity;

            if (!_returning && _rb.position.y < _voidY)
            {
                Destroy(gameObject);
                return;
            }

            if (_returning)
            {
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
                return;
            }

            // Homing behavior: turn toward the nearest enemy every fixed step.
            // Range check gated only when homingRange > 0; 0 means arena-wide.
            if (_data != null && _data.behavior == BallBehavior.Homing)
            {
                Vector2 vel = _rb.linearVelocity;
                if (vel.sqrMagnitude < 0.01f) return;

                var target = FindNearestEnemy();
                if (target == null) return;

                Vector2 toTarget = ((Vector2)target.position - _rb.position);
                if (_data.homingRange > 0f &&
                    toTarget.sqrMagnitude > _data.homingRange * _data.homingRange) return;

                float currentAngle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                float desiredAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle,
                    _data.homingTurnRate * Time.fixedDeltaTime);
                float rad = newAngle * Mathf.Deg2Rad;
                _rb.linearVelocity = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * vel.magnitude;
            }
        }

        private Transform FindNearestEnemy()
        {
            var enemies = FindObjectsByType<Enemy.Enemy>(FindObjectsSortMode.None);
            Transform best = null;
            float bestDist = float.MaxValue;
            Vector2 pos = transform.position;
            foreach (var e in enemies)
            {
                float d = ((Vector2)e.transform.position - pos).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = e.transform; }
            }
            return best;
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
        private int BounceExtensionUpgradeAdd() =>
            Systems.UpgradeApplier.Instance != null ? Systems.UpgradeApplier.Instance.BounceExtensionAdd : 0;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _bounceCount++;

            bool hitEnemy  = collision.gameObject.TryGetComponent<Enemy.Enemy>(out var enemy);
            bool hitWall   = collision.gameObject.GetComponent<Wall>() != null;
            bool hitPlayer = collision.gameObject.GetComponent<Player.PlayerHealth>() != null;

            float damage = (_data != null ? _data.damage : _damage) * DamageUpgradeMul();
            // Relics: Dimensional Fragment (+30%) and Red Thread of Fate (x2 raw dmg)
            if (Systems.RelicSystem.Instance != null)
            {
                if (Systems.RelicSystem.Instance.HasRelic("dim_fragment")) damage *= 1.3f;
                if (Systems.RelicSystem.Instance.HasRelic("red_thread"))    damage *= 2f;
            }
            if (hitEnemy) enemy.TakeDamage(damage);

            // Subtle screen shake on wall/player-body bounce for kinetic feel
            if ((hitWall || hitPlayer) && CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(0.06f);

            if (hitWall || hitPlayer) Audio.AudioManager.Instance?.Play("ball_bounce");
            if (hitEnemy)             Audio.AudioManager.Instance?.Play("ball_hit_enemy");

            // Expanding ring at the contact point sells the bounce
            if ((hitWall || hitPlayer) && collision.contactCount > 0)
            {
                Color ringColor = _sprite != null ? _sprite.color : Color.white;
                ringColor.a = 0.7f;
                Utility.Fx.ImpactRing(collision.GetContact(0).point, ringColor);
            }

            // Charge Sakuya's Time Stop meter — the primary offensive ability feeds off aggression
            if (Systems.TimeStopSystem.Instance != null)
            {
                if (hitWall) Systems.TimeStopSystem.Instance.AddChargeFromWallBounce();
                if (hitEnemy) Systems.TimeStopSystem.Instance.AddChargeFromEnemyHit();
            }

            // Silver Knives relic: on a wall bounce, spit 3 knife-like children
            if (hitWall && !_returning && Systems.RelicSystem.Instance != null &&
                Systems.RelicSystem.Instance.HasRelic("silver_knives"))
            {
                SpawnKniveChildren();
            }

            // Behavior branches
            var behavior = _data != null ? _data.behavior : BallBehavior.Normal;

            // Piercing: don't bounce off enemies — restore pre-collision velocity direction
            if (behavior == BallBehavior.Piercing && hitEnemy)
            {
                _bounceCount--; // undo the count so wall bounces stay the primary trigger
                float pspeed = (_data != null ? _data.speed : _speed) * SpeedUpgradeMul();
                Vector2 dir = _prevVelocity.sqrMagnitude > 0.01f ? _prevVelocity.normalized : _rb.linearVelocity.normalized;
                _rb.linearVelocity = dir * pspeed;
                return;
            }

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

            // Lightning: chain damage to N nearest enemies within range on hit
            if (behavior == BallBehavior.Lightning && hitEnemy && _data != null)
            {
                var hits = Physics2D.OverlapCircleAll(
                    enemy.transform.position, _data.chainRange, _enemyLayers);
                int hopped = 0;
                Vector3 arcFrom = enemy.transform.position;
                foreach (var h in hits)
                {
                    if (hopped >= _data.chainCount) break;
                    if (h.gameObject == enemy.gameObject) continue;
                    if (h.TryGetComponent<Enemy.Enemy>(out var chained))
                    {
                        chained.TakeDamage(_data.chainDamage * DamageUpgradeMul());
                        SpawnLightningArc(arcFrom, chained.transform.position);
                        arcFrom = chained.transform.position;
                        hopped++;
                    }
                }
                Audio.AudioManager.Instance?.Play("ball_hit_enemy", 0.7f, 0.06f);
            }

            // Freeze: slow the hit enemy briefly
            if (behavior == BallBehavior.Freeze && hitEnemy && _data != null)
            {
                var mover = enemy.GetComponent<Enemy.EnemyMover>();
                if (mover != null) mover.ApplySlow(_data.freezeDuration, _data.freezeSlowFactor);
                var sr = enemy.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = Color.Lerp(sr.color, new Color(0.6f, 0.85f, 1f), 0.45f);
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

            // Bouncing off the player's own body is a pure reflect — never destroy/recall here,
            // otherwise a bounce that happens to land on the return-threshold vanishes on contact
            // instead of visibly bouncing.
            if (!hitPlayer)
            {
                int maxB = _data != null ? _data.maxBounces : _maxBounces;
                if (maxB > 0 && _bounceCount >= maxB)
                {
                    Destroy(gameObject);
                    return;
                }

                // Return to player after N bounces (skips for Split children mid-tree to keep them lively)
                int bounceCap = _returnAfterBounces + BounceExtensionUpgradeAdd();
                if (bounceCap > 0 && _bounceCount >= bounceCap && !_returning)
                {
                    EnterReturnMode();
                }
            }
        }

        private void SpawnLightningArc(Vector3 from, Vector3 to)
        {
            var go = new GameObject("LightningArc");
            go.transform.position = from;
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
            lr.startWidth = 0.12f;
            lr.endWidth = 0.05f;
            lr.numCapVertices = 2;
            lr.sortingOrder = 15;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            var grad = new Gradient();
            var c = new Color(0.75f, 0.95f, 1f);
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            lr.colorGradient = grad;
            go.AddComponent<Utility.LightningArcFade>();
        }

        private void SpawnKniveChildren()
        {
            // Fires 3 short-lived mini-balls in a narrow forward fan on wall contact.
            Vector2 baseDir = _rb.linearVelocity.normalized;
            if (baseDir == Vector2.zero) baseDir = Vector2.up;
            for (int i = 0; i < 3; i++)
            {
                float angle = (i - 1) * 12f; // -12, 0, +12 degrees
                Vector2 dir = Quaternion.Euler(0f, 0f, angle) * baseDir;
                var childObj = Instantiate(gameObject, transform.position, Quaternion.identity);
                if (childObj.TryGetComponent<Ball>(out var childBall))
                {
                    // Ensure children can't infinite-recurse via the relic (their bounce triggers spawn again)
                    childBall.SetSplitDepth(_splitDepth + 99);
                    childBall.transform.localScale *= 0.65f;
                    childBall.Launch(dir);
                }
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
