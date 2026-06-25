using UnityEngine;

namespace Tether.CameraSystems
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector2 _offset = new Vector2(0f, 1.5f);

        [Header("Smoothing")]
        [SerializeField] private float _smoothTimeX = 0.18f;
        [SerializeField] private float _smoothTimeY = 0.26f;

        [Header("Deadzone")]
        [SerializeField] private Vector2 _deadzone = new Vector2(1.4f, 1.2f);

        [Header("Look Ahead (horizontal)")]
        [SerializeField] private float _lookAheadDistance = 1.6f;
        [SerializeField] private float _lookAheadSmoothTime = 0.4f;

        [Header("Bounds")]
        [SerializeField] private bool _useBounds = false;
        [SerializeField] private Vector2 _minBounds = new Vector2(-100f, -100f);
        [SerializeField] private Vector2 _maxBounds = new Vector2(100f, 100f);

        private Vector3 _velocity;
        private float _lookAheadX;
        private float _lookAheadVelX;
        private Vector3 _lastTargetPos;
        private bool _hasLastPos;

        public Transform Target { get => _target; set { _target = value; _hasLastPos = false; } }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPos = _target.position;

            float targetVelX = 0f;
            if (_hasLastPos)
            {
                targetVelX = (targetPos.x - _lastTargetPos.x) / Mathf.Max(Time.deltaTime, 0.0001f);
            }
            _lastTargetPos = targetPos;
            _hasLastPos = true;

            float desiredLookAhead = Mathf.Sign(targetVelX) * _lookAheadDistance * Mathf.Clamp01(Mathf.Abs(targetVelX) / 8f);
            _lookAheadX = Mathf.SmoothDamp(_lookAheadX, desiredLookAhead, ref _lookAheadVelX, _lookAheadSmoothTime);

            Vector2 anchor = (Vector2)targetPos + _offset + new Vector2(_lookAheadX, 0f);
            Vector2 camPos = transform.position;

            float desiredX = camPos.x;
            float desiredY = camPos.y;
            float diffX = anchor.x - camPos.x;
            float diffY = anchor.y - camPos.y;

            if (Mathf.Abs(diffX) > _deadzone.x)
            {
                desiredX = anchor.x - Mathf.Sign(diffX) * _deadzone.x;
            }
            if (Mathf.Abs(diffY) > _deadzone.y)
            {
                desiredY = anchor.y - Mathf.Sign(diffY) * _deadzone.y;
            }

            float newX = Mathf.SmoothDamp(camPos.x, desiredX, ref _velocity.x, _smoothTimeX);
            float newY = Mathf.SmoothDamp(camPos.y, desiredY, ref _velocity.y, _smoothTimeY);

            if (_useBounds)
            {
                newX = Mathf.Clamp(newX, _minBounds.x, _maxBounds.x);
                newY = Mathf.Clamp(newY, _minBounds.y, _maxBounds.y);
            }

            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.5f);
            Vector3 c = transform.position;
            Gizmos.DrawWireCube(c, new Vector3(_deadzone.x * 2f, _deadzone.y * 2f, 0f));
        }
    }
}
