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
            // Restore HP from RunSession if we came back from Map with a cache
            var session = Meta.RunSession.Instance;
            if (session != null && session.CachedPlayerHp >= 0)
            {
                _maxHp = session.CachedPlayerMaxHp > 0 ? session.CachedPlayerMaxHp : _maxHp;
                CurrentHp = Mathf.Clamp(session.CachedPlayerHp, 1, _maxHp);
            }
            else
            {
                CurrentHp = _maxHp;
            }
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

        /// Force invuln on/off (used by dash). Independent of the flash coroutine.
        public void SetInvulnerable(bool on)
        {
            IsInvulnerable = on;
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            CurrentHp = Mathf.Min(_maxHp, CurrentHp + amount);
            OnDamaged?.Invoke(CurrentHp, _maxHp);
        }

        public void ExtendMaxHp(int add)
        {
            if (add <= 0) return;
            _maxHp += add;
            CurrentHp += add; // also grant the extra HP up-front
            OnDamaged?.Invoke(CurrentHp, _maxHp);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead || IsInvulnerable) return;
            ApplyDamage(damage);
        }

        /// <summary>Same as TakeDamage but ignores the post-hit invulnerability window.
        /// Each escaped enemy is its own discrete punishment, not repeated contact from
        /// the same source — several escaping in the same beat (e.g. a spawned row)
        /// must each land instead of the first one shielding the rest.</summary>
        public void TakeEscapeDamage(int damage)
        {
            if (IsDead) return;
            ApplyDamage(damage);
        }

        private void ApplyDamage(int damage)
        {
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
