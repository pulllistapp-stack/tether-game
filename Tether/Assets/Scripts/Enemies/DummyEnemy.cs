using System.Collections;
using UnityEngine;
using Tether.Utility;
using Tether.CameraSystems;

namespace Tether.Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DummyEnemy : MonoBehaviour
    {
        [SerializeField] private float _hitFlashDuration = 0.08f;
        [SerializeField] private float _hitStopDuration = 0.06f;
        [SerializeField] private float _shakeAmount = 0.45f;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _knockbackForce = 6f;

        private SpriteRenderer _sr;
        private Color _baseColor;
        private Rigidbody2D _rb;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _baseColor = _sr.color;
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Hit(Vector2 sourcePosition)
        {
            if (HitStop.Instance != null) HitStop.Instance.Freeze(_hitStopDuration);
            if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(_shakeAmount);

            if (_rb != null && _rb.bodyType == RigidbodyType2D.Dynamic)
            {
                Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
                if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
                dir.y = Mathf.Max(dir.y, 0.35f);
                _rb.linearVelocity = dir.normalized * _knockbackForce;
            }

            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _sr.color = _flashColor;
            float elapsed = 0f;
            while (elapsed < _hitFlashDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            _sr.color = _baseColor;
            _flashRoutine = null;
        }
    }
}
