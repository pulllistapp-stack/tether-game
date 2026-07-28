using UnityEngine;
using System.Collections;

namespace Tether.Player
{
    /// <summary>
    /// Phase 4 dash. Space key: quickly translate along last movement input
    /// (falls back to horizontal input at press-time). Grants brief
    /// invulnerability via PlayerHealth and has a cooldown.
    /// </summary>
    public class PlayerDash : MonoBehaviour
    {
        [Header("Dash")]
        [SerializeField] private KeyCode _dashKey = KeyCode.Space;
        [SerializeField] private float _dashDistance = 3.2f;
        [SerializeField] private float _dashDuration = 0.16f;
        [SerializeField] private float _cooldown = 1.4f;

        [Header("Bounds (world units)")]
        [SerializeField] private Vector2 _boundsMin = new Vector2(-5.5f, -7f);
        [SerializeField] private Vector2 _boundsMax = new Vector2(5.5f, 6.5f);

        public float Cooldown => _cooldown;
        public float TimeSinceLastDash => Time.time - _lastDashAt;
        public bool IsReady => TimeSinceLastDash >= _cooldown;

        private PlayerHealth _hp;
        private Coroutine _routine;
        private float _lastDashAt = -999f;
        private Vector2 _lastMoveDir = Vector2.right;

        private void Awake()
        {
            _hp = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 0.01f) _lastMoveDir = input.normalized;

            if (Input.GetKeyDown(_dashKey) && IsReady && _routine == null)
            {
                Vector2 dir = input.sqrMagnitude > 0.01f ? input.normalized : _lastMoveDir;
                _routine = StartCoroutine(DashRoutine(dir));
            }
        }

        private IEnumerator DashRoutine(Vector2 dir)
        {
            _lastDashAt = Time.time;
            if (_hp != null) _hp.SetInvulnerable(true);

            Audio.AudioManager.Instance?.Play("ball_fire", 0.6f, 0.02f); // reuse chirp
            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(0.08f);

            Vector3 start = transform.position;
            Vector3 target = start + (Vector3)(dir * _dashDistance);
            target.x = Mathf.Clamp(target.x, _boundsMin.x, _boundsMax.x);
            target.y = Mathf.Clamp(target.y, _boundsMin.y, _boundsMax.y);

            float t = 0f;
            while (t < _dashDuration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / _dashDuration);
                float eased = 1f - (1f - u) * (1f - u); // ease-out quad
                transform.position = Vector3.Lerp(start, target, eased);
                yield return null;
            }
            transform.position = target;

            if (_hp != null) _hp.SetInvulnerable(false);
            _routine = null;
        }
    }
}
