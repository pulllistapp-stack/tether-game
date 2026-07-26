using UnityEngine;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 5 enemy projectile. Straight-line travel; on player overlap
    /// damages PlayerHealth. Also destroyed on wall contact.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 6f;
        [SerializeField] private float _damage = 1f;

        private Rigidbody2D _rb;
        private float _spawnTime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.freezeRotation = true;
            _spawnTime = Time.time;
        }

        public void Launch(Vector2 dir, float speed, float damage)
        {
            _damage = damage;
            _rb.linearVelocity = dir.normalized * speed;
        }

        private void Update()
        {
            if (Time.time - _spawnTime > _lifetime) Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Bounce off walls — nope, destroy for cleanliness
            if (collision.gameObject.GetComponent<Gameplay.Wall>() != null)
            {
                Destroy(gameObject);
            }
        }

        // Player uses trigger collider — projectile is dynamic non-trigger,
        // so the trigger fires on Player's OnTriggerEnter2D. We hook there
        // via PlayerHealth polling (already OverlapCircle-based), OR we
        // handle it here by polling too.
        private void FixedUpdate()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO == null) return;
            var hp = playerGO.GetComponent<Player.PlayerHealth>();
            if (hp == null) return;
            if (Vector2.Distance(transform.position, playerGO.transform.position) <= 0.4f)
            {
                hp.TakeDamage(Mathf.Max(1, Mathf.RoundToInt(_damage)));
                Destroy(gameObject);
            }
        }
    }
}
