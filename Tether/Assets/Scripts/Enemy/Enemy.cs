using UnityEngine;
using System;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 1 baseline Enemy. HP + damage receiving + destroy on death.
    /// Extended later with AI, movement patterns, wave rewards.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHp = 5f;

        [Header("Feel (on death)")]
        [SerializeField] private float _deathShake = 0.4f;
        [SerializeField] private float _deathHitStop = 0.04f;

        public event Action<Enemy> OnDeath;

        private float _currentHp;

        private void Awake()
        {
            _currentHp = _maxHp;
        }

        public void TakeDamage(float damage)
        {
            _currentHp -= damage;
            if (_currentHp <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke(this);

            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(_deathShake);
            if (Utility.HitStop.Instance != null)
                Utility.HitStop.Instance.Freeze(_deathHitStop);

            SpawnDeathParticles();
            Destroy(gameObject);
        }

        private void SpawnDeathParticles()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) return;

            const int count = 5;
            for (int i = 0; i < count; i++)
            {
                var pGO = new GameObject("DeathParticle");
                pGO.transform.position = transform.position;
                pGO.transform.localScale = Vector3.one * 0.35f;
                var psr = pGO.AddComponent<SpriteRenderer>();
                psr.sprite = sr.sprite;
                psr.color = sr.color;
                psr.sortingOrder = sr.sortingOrder + 1;
                var rb = pGO.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.linearDamping = 4f;
                float angle = (360f / count) * i + UnityEngine.Random.Range(-15f, 15f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                rb.linearVelocity = dir * UnityEngine.Random.Range(3f, 5f);
                rb.angularVelocity = UnityEngine.Random.Range(-360f, 360f);
                pGO.AddComponent<Utility.FadeAndDestroy>();
            }
        }
    }
}
