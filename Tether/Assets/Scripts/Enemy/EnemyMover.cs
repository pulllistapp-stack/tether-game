using UnityEngine;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 2 enemy AI. Drifts downward with slight horizontal homing on the player.
    /// Requires Kinematic Rigidbody2D so it triggers overlap events on the Player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMover : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float _fallSpeed = 0.6f;
        [Tooltip("How strongly the enemy homes horizontally toward the player (0=no home, 1=snap).")]
        [Range(0f, 1f)]
        [SerializeField] private float _homingStrength = 0.25f;

        [Header("Runtime scale (set by WaveSystem)")]
        [SerializeField] private float _speedMultiplier = 1f;

        private Rigidbody2D _rb;
        private Transform _playerTf;

        public void SetSpeedMultiplier(float mul) => _speedMultiplier = mul;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) _playerTf = playerGO.transform;
        }

        private void FixedUpdate()
        {
            Vector2 pos = _rb.position;

            float dx = 0f;
            if (_playerTf != null)
            {
                dx = Mathf.Sign(_playerTf.position.x - pos.x) * _homingStrength;
            }

            Vector2 velocity = new Vector2(dx, -1f).normalized * _fallSpeed * _speedMultiplier;
            _rb.MovePosition(pos + velocity * Time.fixedDeltaTime);
        }
    }
}
