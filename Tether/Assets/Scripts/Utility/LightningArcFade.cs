using UnityEngine;

namespace Tether.Utility
{
    /// <summary>Quickly fades a LineRenderer's alpha to 0 and destroys the GameObject.</summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LightningArcFade : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.15f;

        private LineRenderer _lr;
        private float _elapsed;

        private void Awake() { _lr = GetComponent<LineRenderer>(); }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            var grad = _lr.colorGradient;
            var newG = new Gradient();
            newG.SetKeys(grad.colorKeys, new GradientAlphaKey[] {
                new GradientAlphaKey(1f - t, 0f),
                new GradientAlphaKey(0f, 1f)
            });
            _lr.colorGradient = newG;
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
