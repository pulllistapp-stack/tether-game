using UnityEngine;

namespace Tether.Gameplay
{
    /// <summary>
    /// Phase 1 baseline Ball. Bounces off walls/enemies via Rigidbody2D physics.
    /// Deals damage on contact. Extended later for Fusion / Baby ball / Character combo.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Ball : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] private float _speed = 12f;
        [Tooltip("If true, ball keeps constant speed after every bounce.")]
        [SerializeField] private bool _preserveSpeedOnBounce = true;

        [Header("Combat")]
        [SerializeField] private float _damage = 1f;

        [Header("Lifetime")]
        [Tooltip("-1 = infinite bounces until caught / lost.")]
        [SerializeField] private int _maxBounces = -1;

        private Rigidbody2D _rb;
        private int _bounceCount;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.freezeRotation = true;
        }

        public void Launch(Vector2 direction)
        {
            _rb.linearVelocity = direction.normalized * _speed;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _bounceCount++;

            if (collision.gameObject.TryGetComponent<Enemy.Enemy>(out var enemy))
            {
                enemy.TakeDamage(_damage);
            }

            if (_preserveSpeedOnBounce)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _speed;
            }

            if (_maxBounces > 0 && _bounceCount >= _maxBounces)
            {
                Destroy(gameObject);
            }
        }
    }
}
