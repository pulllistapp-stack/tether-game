using UnityEngine;

namespace Tether.Utility
{
    /// <summary>
    /// Fades a SpriteRenderer's alpha to 0 over _duration, then destroys the GameObject.
    /// Used by transient death-particle sprites.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FadeAndDestroy : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.35f;

        private SpriteRenderer _sr;
        private float _elapsed;
        private Color _startColor;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _startColor = _sr.color;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            var c = _startColor;
            c.a = Mathf.Lerp(_startColor.a, 0f, t);
            _sr.color = c;
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
