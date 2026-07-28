using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Tether.Utility
{
    /// <summary>
    /// Attaches a point Light2D that follows the host's SpriteRenderer colour.
    ///
    /// Put this on anything that should read as a light source — balls, XP orbs,
    /// coins, the boss. Because the light tints itself from the sprite, a Freeze
    /// ball glows cold and an Explosive ball glows hot with no extra setup.
    /// </summary>
    [DisallowMultipleComponent]
    public class AutoLight2D : MonoBehaviour
    {
        [SerializeField] private float _radius = 2.2f;
        [SerializeField] private float _intensity = 1.1f;
        [Tooltip("Blend between the sprite's own colour and white. 0 = pure sprite colour.")]
        [Range(0f, 1f)]
        [SerializeField] private float _whiten = 0.25f;
        [Tooltip("Sine pulse depth. 0 disables the pulse.")]
        [SerializeField] private float _pulseAmount = 0.12f;
        [SerializeField] private float _pulseSpeed = 6f;

        private Light2D _light;
        private SpriteRenderer _sprite;
        private float _baseIntensity;
        private float _phase;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _phase = Random.value * Mathf.PI * 2f; // desync pulses between objects

            var lightGO = new GameObject("Light2D");
            lightGO.transform.SetParent(transform, false);

            _light = lightGO.AddComponent<Light2D>();
            _light.lightType = Light2D.LightType.Point;
            _light.pointLightInnerRadius = _radius * 0.25f;
            _light.pointLightOuterRadius = _radius;
            _light.intensity = _intensity;
            _light.falloffIntensity = 0.55f;

            _baseIntensity = _intensity;
            ApplyColor();
        }

        private void LateUpdate()
        {
            if (_light == null) return;

            ApplyColor();

            if (_pulseAmount > 0f)
            {
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * _pulseSpeed + _phase) * _pulseAmount;
                _light.intensity = _baseIntensity * pulse;
            }

            // The parent may be scaled (boss = 3.5x); undo it so light radius is
            // authored in world units rather than inheriting sprite scale.
            float parentScale = Mathf.Max(0.01f, transform.lossyScale.x);
            _light.transform.localScale = Vector3.one / parentScale;
        }

        private void ApplyColor()
        {
            if (_sprite == null) return;
            _light.color = Color.Lerp(_sprite.color, Color.white, _whiten);
        }

        public void Configure(float radius, float intensity)
        {
            _radius = radius;
            _intensity = intensity;
            _baseIntensity = intensity;
            if (_light != null)
            {
                _light.pointLightOuterRadius = radius;
                _light.pointLightInnerRadius = radius * 0.25f;
                _light.intensity = intensity;
            }
        }
    }
}
