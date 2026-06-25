using System.Collections;
using UnityEngine;

namespace Tether.CameraSystems
{
    public class CameraShaker : MonoBehaviour
    {
        public static CameraShaker Instance { get; private set; }

        [SerializeField] private float _trauma = 0f;
        [SerializeField] private float _traumaDecay = 1.8f;
        [SerializeField] private float _maxOffsetX = 0.25f;
        [SerializeField] private float _maxOffsetY = 0.18f;
        [SerializeField] private float _maxRotation = 4f;
        [SerializeField] private float _frequency = 28f;

        private Vector3 _basePosition;
        private float _seedX;
        private float _seedY;
        private float _seedR;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _basePosition = transform.localPosition;
            _seedX = Random.value * 1000f;
            _seedY = Random.value * 1000f;
            _seedR = Random.value * 1000f;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Shake(float amount)
        {
            _trauma = Mathf.Clamp01(_trauma + amount);
        }

        private void LateUpdate()
        {
            if (_trauma <= 0.001f)
            {
                transform.localPosition = _basePosition;
                transform.localRotation = Quaternion.identity;
                return;
            }

            float shake = _trauma * _trauma;
            float t = Time.unscaledTime * _frequency;

            float ox = (Mathf.PerlinNoise(_seedX, t) - 0.5f) * 2f * _maxOffsetX * shake;
            float oy = (Mathf.PerlinNoise(_seedY, t) - 0.5f) * 2f * _maxOffsetY * shake;
            float or = (Mathf.PerlinNoise(_seedR, t) - 0.5f) * 2f * _maxRotation * shake;

            transform.localPosition = _basePosition + new Vector3(ox, oy, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, or);

            _trauma = Mathf.Max(0f, _trauma - _traumaDecay * Time.unscaledDeltaTime);
        }
    }
}
