using UnityEngine;
using System;
using System.Collections;

namespace Tether.Player
{
    /// <summary>
    /// Phase 2 player health. Polls Physics2D.OverlapCircle for enemy contact
    /// (avoids trigger-matrix pitfalls between Kinematic RBs and no-RB colliders).
    /// Emits OnDamaged / OnDied events; broadcasts current/max HP for HUD polling.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int _maxHp = 5;
        [SerializeField] private int _contactDamage = 1;
        [SerializeField] private float _invulnDuration = 0.7f;

        [Header("Contact Detection")]
        [SerializeField] private float _hitboxRadius = 0.35f;
        [SerializeField] private LayerMask _enemyLayers = ~0;

        [Header("Feedback")]
        [SerializeField] private SpriteRenderer _flashRenderer;
        [SerializeField] private Color _flashColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private float _flashInterval = 0.08f;

        public int MaxHp => _maxHp;
        public int CurrentHp { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInvulnerable { get; private set; }

        public event Action<int, int> OnDamaged;
        public event Action OnDied;

        private Color _baseColor;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            CurrentHp = _maxHp;
            if (_flashRenderer != null) _baseColor = _flashRenderer.color;
        }

        private void FixedUpdate()
        {
            if (IsDead || IsInvulnerable) return;

            var hits = Physics2D.OverlapCircleAll(transform.position, _hitboxRadius, _enemyLayers);
            foreach (var h in hits)
            {
                if (h.GetComponentInParent<Enemy.Enemy>() != null)
                {
                    TakeDamage(_contactDamage);
                    break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.4f, 0.4f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, _hitboxRadius);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead || IsInvulnerable) return;
            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            OnDamaged?.Invoke(CurrentHp, _maxHp);

            if (CurrentHp <= 0)
            {
                Die();
                return;
            }

            if (_flashRenderer != null)
            {
                if (_flashRoutine != null) StopCoroutine(_flashRoutine);
                _flashRoutine = StartCoroutine(FlashRoutine());
            }
        }

        private void Die()
        {
            IsDead = true;
            OnDied?.Invoke();
        }

        private IEnumerator FlashRoutine()
        {
            IsInvulnerable = true;
            float elapsed = 0f;
            bool flashed = false;
            while (elapsed < _invulnDuration)
            {
                _flashRenderer.color = flashed ? _baseColor : _flashColor;
                flashed = !flashed;
                yield return new WaitForSeconds(_flashInterval);
                elapsed += _flashInterval;
            }
            _flashRenderer.color = _baseColor;
            IsInvulnerable = false;
            _flashRoutine = null;
        }
    }
}
