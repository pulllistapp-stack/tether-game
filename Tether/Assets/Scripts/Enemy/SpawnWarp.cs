using UnityEngine;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 8 spawn-in effect. Fades alpha 0→1 and scales up past the target
    /// before settling, so enemies "warp in" instead of blinking into existence.
    /// Self-removes once finished — zero cost after the first quarter second.
    /// </summary>
    public class SpawnWarp : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.28f;
        [SerializeField] private float _startScaleFactor = 0.35f;
        [SerializeField] private float _overshoot = 1.12f;

        private SpriteRenderer _sr;
        private Vector3 _targetScale;
        private Color _targetColor;
        private float _elapsed;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Start() (not Awake) so Enemy.Configure has already applied the final
            // scale + tint; we animate toward whatever it settled on.
            _targetScale = transform.localScale;
            if (_sr != null)
            {
                _targetColor = _sr.color;
                var c = _targetColor; c.a = 0f;
                _sr.color = c;
            }
            transform.localScale = _targetScale * _startScaleFactor;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            // Overshoot then settle
            float scaleMul = t < 0.65f
                ? Mathf.Lerp(_startScaleFactor, _overshoot, t / 0.65f)
                : Mathf.Lerp(_overshoot, 1f, (t - 0.65f) / 0.35f);
            transform.localScale = _targetScale * scaleMul;

            if (_sr != null)
            {
                var c = _targetColor;
                c.a = _targetColor.a * Mathf.Clamp01(t * 2f); // fade in faster than the scale
                _sr.color = c;
            }

            if (t >= 1f)
            {
                transform.localScale = _targetScale;
                if (_sr != null) _sr.color = _targetColor;
                Destroy(this);
            }
        }
    }
}
