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
        [Tooltip("Minimum Y component of aim direction (0..1). Clamps aim to upper hemisphere.")]
        [Range(0f, 1f)]
        [SerializeField] private float _minAimY = 0.15f;

        private Camera _camera;
        private float _lastFireTime;
        private BallSlotManager _slots;

        private void Awake()
        {
            _camera = Camera.main;
            if (_firePoint == null)
            {
                _firePoint = transform;
            }
            _slots = GetComponent<BallSlotManager>();
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
            Vector2 dir = ((Vector2)(mouseWorld - _firePoint.position)).normalized;
            if (dir.y < _minAimY)
            {
                dir.y = _minAimY;
                dir = dir.normalized;
            }
            return dir;
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
            float cooldownMul = Systems.UpgradeApplier.Instance != null
                ? Systems.UpgradeApplier.Instance.FireCooldownMul : 1f;
            int extras = Systems.UpgradeApplier.Instance != null
                ? Systems.UpgradeApplier.Instance.ExtraProjectiles : 0;

            if (Time.time - _lastFireTime < _fireCooldown * cooldownMul) return;
            if (_ballPrefab == null) return;

            _lastFireTime = Time.time;

            Audio.AudioManager.Instance?.Play("ball_fire");

            int total = 1 + Mathf.Max(0, extras);
            const float spreadDegPerExtra = 8f;
            float halfSpread = (total - 1) * spreadDegPerExtra * 0.5f;
            for (int i = 0; i < total; i++)
            {
                float angleOffset = total == 1 ? 0f : -halfSpread + i * spreadDegPerExtra;
                Vector2 shotDir = Quaternion.Euler(0f, 0f, angleOffset) * dir;
                SpawnAndLaunchBall(shotDir);
            }
        }

        private void SpawnAndLaunchBall(Vector2 dir)
        {
            var ballObj = Instantiate(_ballPrefab, _firePoint.position, Quaternion.identity);
            if (ballObj.TryGetComponent<Gameplay.Ball>(out var ball))
            {
                if (_slots != null && _slots.CurrentData != null)
                    ball.Configure(_slots.CurrentData);
                ball.Launch(dir);
            }
        }
    }
}
