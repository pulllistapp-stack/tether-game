using UnityEngine;

namespace Tether.UI
{
    /// <summary>
    /// Drives post-processing from gameplay state.
    ///
    /// Two jobs: the vignette tightens as HP drops so danger is readable at the
    /// edge of vision without looking at the HUD, and chromatic aberration
    /// pulses on hit. Both read from SceneLightingBootstrap, which owns the
    /// runtime volume.
    /// </summary>
    public class ScreenFeedback : MonoBehaviour
    {
        [Header("Low-HP vignette")]
        [Tooltip("HP fraction at or below which the vignette starts closing in.")]
        [Range(0f, 1f)]
        [SerializeField] private float _dangerThreshold = 0.4f;
        [SerializeField] private float _maxVignette = 0.55f;
        [Tooltip("Extra pulse depth at critical HP.")]
        [SerializeField] private float _pulseAmount = 0.07f;
        [SerializeField] private float _pulseSpeed = 3.2f;

        [Header("Hit flash")]
        [SerializeField] private float _hitChromatic = 0.7f;
        [SerializeField] private float _hitDecay = 2.5f;

        private Player.PlayerHealth _health;
        private float _chromatic;
        private int _lastHp = -1;

        private void Start()
        {
            TryBind();
        }

        private void OnDestroy()
        {
            if (_health != null) _health.OnDamaged -= HandleDamaged;
        }

        private void TryBind()
        {
            if (_health != null) return;
            _health = FindFirstObjectByType<Player.PlayerHealth>();
            if (_health != null)
            {
                _lastHp = _health.CurrentHp;
                _health.OnDamaged += HandleDamaged;
            }
        }

        private void HandleDamaged(int current, int max)
        {
            // OnDamaged also fires on heal / max-HP grants, so only flash on a real loss
            if (_lastHp >= 0 && current < _lastHp) _chromatic = _hitChromatic;
            _lastHp = current;
        }

        private void Update()
        {
            if (_health == null) { TryBind(); return; }

            var fx = Systems.SceneLightingBootstrap.Instance;
            if (fx == null) return;

            // --- Vignette from HP ---
            float hp01 = _health.MaxHp > 0 ? (float)_health.CurrentHp / _health.MaxHp : 1f;
            float target = fx.BaseVignette;

            if (hp01 <= _dangerThreshold)
            {
                // 0 at the threshold, 1 at zero HP
                float danger = 1f - Mathf.Clamp01(hp01 / Mathf.Max(0.0001f, _dangerThreshold));
                target = Mathf.Lerp(fx.BaseVignette, _maxVignette, danger);
                target += Mathf.Sin(Time.unscaledTime * _pulseSpeed) * _pulseAmount * danger;
            }
            fx.SetVignette(target);

            // --- Chromatic decay ---
            if (_chromatic > 0f)
            {
                _chromatic = Mathf.Max(0f, _chromatic - _hitDecay * Time.unscaledDeltaTime);
                fx.SetChromatic(_chromatic);
            }
        }
    }
}
