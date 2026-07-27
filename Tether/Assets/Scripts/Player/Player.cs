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
        [Tooltip("If true, the player continuously auto-fires whenever the cooldown is ready. Aim still follows the mouse.")]
        [SerializeField] private bool _autoFire = true;

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
            if (Systems.RunController.IsRunOver) return;

            Vector2 aimDir = GetAimDirection();
            UpdateAimLine(aimDir);

            bool wantsFire = _autoFire || Input.GetMouseButton(0);
            if (wantsFire)
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
            // Red Thread of Fate relic slows fire rate significantly (paired with damage buff on Ball side)
            if (Systems.RelicSystem.Instance != null && Systems.RelicSystem.Instance.HasRelic("red_thread"))
                cooldownMul *= 2f;
            int extras = Systems.UpgradeApplier.Instance != null
                ? Systems.UpgradeApplier.Instance.ExtraProjectiles : 0;

            if (Time.time - _lastFireTime < _fireCooldown * cooldownMul) return;
            if (_ballPrefab == null) return;
            // Empty-handed: don't burn the cooldown or play the fire sound on a dry attempt —
            // the next real shot fires the instant a ball comes back instead of queuing behind it.
            if (_slots == null || _slots.AvailableCount <= 0) return;

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
            if (_slots == null) return;

            // Fixed 5-ball hand: no free slot means no shot this attempt, full stop.
            int slotIndex = _slots.ReserveNextAvailable();
            if (slotIndex < 0) return;

            var ballObj = Instantiate(_ballPrefab, _firePoint.position, Quaternion.identity);
            if (ballObj.TryGetComponent<Gameplay.Ball>(out var ball))
            {
                var data = _slots.DataAt(slotIndex);
                if (data != null) ball.Configure(data);
                ball.SetHomeSlot(_slots, slotIndex);
                ball.Launch(dir);
            }
            else
            {
                _slots.Release(slotIndex);
            }
        }
    }
}
