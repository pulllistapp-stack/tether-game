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
            Destroy(gameObject);
        }
    }
}
