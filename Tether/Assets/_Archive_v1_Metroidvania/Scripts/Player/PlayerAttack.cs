using UnityEngine;
using UnityEngine.InputSystem;
using Tether.Enemies;

namespace Tether.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack Hitbox")]
        [SerializeField] private Vector2 _hitboxSize = new Vector2(1.8f, 2.2f);
        [SerializeField] private Vector2 _hitboxOffset = new Vector2(1.2f, 0.1f);
        [SerializeField] private LayerMask _enemyLayer = ~0;
        [SerializeField] private float _attackCooldown = 0.32f;
        [SerializeField] private float _hitboxDelay = 0.08f;

        [Header("Slash Visual")]
        [SerializeField] private Sprite _slashSprite;
        [SerializeField] private float _slashDuration = 0.12f;
        [SerializeField] private Color _slashColor = new Color(1f, 0.95f, 0.78f, 0.95f);
        [SerializeField] private int _slashSortingOrder = 10;
        [SerializeField] private float _slashScaleBoost = 1.15f;

        [Header("Self Recoil")]
        [SerializeField] private float _selfPushBack = 1.6f;
        [SerializeField] private bool _applySelfRecoil = true;

        [Header("Debug")]
        [SerializeField] private bool _drawGizmos = true;

        private float _cooldownTimer;
        private int _facing = 1;
        private SpriteRenderer _sr;
        private Rigidbody2D _rb;
        private PlayerController _pc;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _pc = GetComponent<PlayerController>();
        }

        private void Update()
        {
            _cooldownTimer -= Time.deltaTime;
        }

        public void OnMove(InputValue value)
        {
            float x = value.Get<Vector2>().x;
            if (Mathf.Abs(x) > 0.1f)
            {
                _facing = x > 0 ? 1 : -1;
            }
        }

        public void OnAttack(InputValue value)
        {
            if (!value.isPressed) return;
            if (_cooldownTimer > 0f) return;
            _cooldownTimer = _attackCooldown;

            if (_pc != null) _pc.TriggerAttackAnim();
            StartCoroutine(DelayedHit());
        }

        private System.Collections.IEnumerator DelayedHit()
        {
            yield return new WaitForSeconds(_hitboxDelay);
            DoAttack();
        }

        private void DoAttack()
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(_hitboxOffset.x * _facing, _hitboxOffset.y);

            SpawnSlashVisual(origin);

            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, _hitboxSize, 0f, _enemyLayer);
            bool connected = false;
            foreach (var hit in hits)
            {
                if (hit == null) continue;
                var enemy = hit.GetComponentInParent<DummyEnemy>();
                if (enemy != null)
                {
                    enemy.Hit(transform.position);
                    connected = true;
                }
            }

            if (connected && _applySelfRecoil && _rb != null)
            {
                _rb.linearVelocity = new Vector2(-_facing * _selfPushBack, _rb.linearVelocity.y);
            }
        }

        private void SpawnSlashVisual(Vector2 origin)
        {
            GameObject fx = new GameObject("SlashFX");
            fx.transform.position = origin;
            fx.transform.localScale = new Vector3(_hitboxSize.x * _slashScaleBoost, _hitboxSize.y * _slashScaleBoost, 1f);
            fx.transform.rotation = Quaternion.Euler(0f, 0f, _facing < 0 ? 180f : 0f);
            var sr = fx.AddComponent<SpriteRenderer>();
            sr.sprite = _slashSprite;
            sr.color = _slashColor;
            sr.sortingOrder = _slashSortingOrder;
            Destroy(fx, _slashDuration);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;
            Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.65f);
            Vector2 origin = (Vector2)transform.position + new Vector2(_hitboxOffset.x * _facing, _hitboxOffset.y);
            Gizmos.DrawWireCube(origin, _hitboxSize);
        }
    }
}
