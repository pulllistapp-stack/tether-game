using UnityEngine;

namespace Tether.Player
{
    /// <summary>
    /// Phase 1 baseline Player. Mouse aim + click to fire.
    /// Uses legacy Input for speed; migrate to Input System actions in Phase 2.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Fire")]
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireCooldown = 0.4f;

        [Header("Aim (optional visual)")]
        [SerializeField] private LineRenderer _aimLine;
        [SerializeField] private float _aimLineLength = 3f;

        private Camera _camera;
        private float _lastFireTime;

        private void Awake()
        {
            _camera = Camera.main;
            if (_firePoint == null)
            {
                _firePoint = transform;
            }
        }

        private void Update()
        {
            Vector2 aimDir = GetAimDirection();
            UpdateAimLine(aimDir);

            if (Input.GetMouseButtonDown(0))
            {
                TryFire(aimDir);
            }
        }

        private Vector2 GetAimDirection()
        {
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            return ((Vector2)(mouseWorld - _firePoint.position)).normalized;
        }

        private void UpdateAimLine(Vector2 dir)
        {
            if (_aimLine == null) return;
            _aimLine.positionCount = 2;
            _aimLine.SetPosition(0, _firePoint.position);
            _aimLine.SetPosition(1, _firePoint.position + (Vector3)(dir * _aimLineLength));
        }

        private void TryFire(Vector2 dir)
        {
            if (Time.time - _lastFireTime < _fireCooldown) return;
            if (_ballPrefab == null) return;

            _lastFireTime = Time.time;

            var ballObj = Instantiate(_ballPrefab, _firePoint.position, Quaternion.identity);
            if (ballObj.TryGetComponent<Gameplay.Ball>(out var ball))
            {
                ball.Launch(dir);
            }
        }
    }
}
