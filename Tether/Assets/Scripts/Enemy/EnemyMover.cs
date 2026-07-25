using UnityEngine;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 3 config-driven mover. Reads EnemyData for behavior; falls back
    /// to inspector defaults when no data is set. Requires Kinematic Rigidbody2D.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMover : MonoBehaviour
    {
        [Header("Fallback config")]
        [SerializeField] private float _fallSpeed = 0.6f;
        [Range(0f, 1f)]
        [SerializeField] private float _homingStrength = 0.25f;
        [SerializeField] private EnemyBehavior _behavior = EnemyBehavior.FallHoming;
        [SerializeField] private float _sineAmplitude = 1.5f;
        [SerializeField] private float _sineFrequency = 0.6f;

        [Header("Runtime scaling (set by WaveSystem)")]
        [SerializeField] private float _speedMultiplier = 1f;

        private Rigidbody2D _rb;
        private Transform _playerTf;
        private float _sineSeedX;
        private float _spawnTime;
        private float _floatY = 3.5f;
        private float _floatBobAmp = 0.6f;
        private float _floatDriftAmp = 2.5f;

        public void SetSpeedMultiplier(float mul) => _speedMultiplier = mul;

        public void ApplyData(EnemyData data)
        {
            if (data == null) return;
            _fallSpeed = data.fallSpeed;
            _homingStrength = data.homingStrength;
            _behavior = data.behavior;
            _sineAmplitude = data.sineAmplitude;
            _sineFrequency = data.sineFrequency;
            _floatY = data.floatY;
            _floatBobAmp = data.floatBobAmp;
            _floatDriftAmp = data.floatDriftAmp;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _sineSeedX = transform.position.x;
            _spawnTime = Time.time;
        }

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) _playerTf = playerGO.transform;
        }

        private void FixedUpdate()
        {
            Vector2 pos = _rb.position;
            Vector2 velocity;

            switch (_behavior)
            {
                case EnemyBehavior.FallStraight:
                    velocity = Vector2.down * _fallSpeed * _speedMultiplier;
                    break;

                case EnemyBehavior.Sidewind:
                {
                    float t = Time.time - _spawnTime;
                    float targetX = _sineSeedX + Mathf.Sin(t * Mathf.PI * 2f * _sineFrequency) * _sineAmplitude;
                    float dx = Mathf.Clamp(targetX - pos.x, -1f, 1f);
                    velocity = new Vector2(dx * _fallSpeed * 2f, -_fallSpeed) * _speedMultiplier;
                    break;
                }

                case EnemyBehavior.FloatSteady:
                {
                    float t = Time.time - _spawnTime;
                    float targetY = _floatY + Mathf.Sin(t * 1.2f) * _floatBobAmp;
                    float targetX = Mathf.Sin(t * 0.5f) * _floatDriftAmp;
                    Vector2 target = new Vector2(targetX, targetY);
                    Vector2 delta = target - pos;
                    velocity = delta * 1.5f * _speedMultiplier;
                    break;
                }

                default: // FallHoming
                {
                    float dx = 0f;
                    if (_playerTf != null)
                        dx = Mathf.Sign(_playerTf.position.x - pos.x) * _homingStrength;
                    velocity = new Vector2(dx, -1f).normalized * _fallSpeed * _speedMultiplier;
                    break;
                }
            }

            _rb.MovePosition(pos + velocity * Time.fixedDeltaTime);
        }
    }
}
