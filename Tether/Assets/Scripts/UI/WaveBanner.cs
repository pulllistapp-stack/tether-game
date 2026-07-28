using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Tether.UI
{
    /// <summary>
    /// Phase 8 wave announce banner. Slides a big label in from the left, holds,
    /// then slides out right. Runs on unscaled time so it still plays if a
    /// hitstop or pause lands mid-animation.
    /// </summary>
    public class WaveBanner : MonoBehaviour
    {
        [SerializeField] private Text _label;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _slideInDuration = 0.35f;
        [SerializeField] private float _holdDuration = 0.9f;
        [SerializeField] private float _slideOutDuration = 0.3f;
        [SerializeField] private float _travelDistance = 900f;

        private Systems.WaveSystem _waveSystem;
        private RectTransform _rt;
        private Coroutine _routine;

        private void Awake()
        {
            _rt = _label != null ? _label.rectTransform : GetComponent<RectTransform>();
            if (_group != null) _group.alpha = 0f;
        }

        private void Start()
        {
            _waveSystem = FindFirstObjectByType<Systems.WaveSystem>();
            if (_waveSystem != null) _waveSystem.OnWaveStarted += HandleWaveStarted;
        }

        private void OnDestroy()
        {
            if (_waveSystem != null) _waveSystem.OnWaveStarted -= HandleWaveStarted;
        }

        private void HandleWaveStarted(int index, int total)
        {
            string text = "WAVE  " + (index + 1);

            // Final wave of a node reads as the boss call-out
            var session = Meta.RunSession.Instance;
            if (session != null && session.CurrentNode != null &&
                session.CurrentNode.type == Meta.NodeType.Boss)
                text = "☠  BOSS  ☠";

            Show(text);
        }

        public void Show(string text)
        {
            if (_label == null || _group == null) return;
            _label.text = text;
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(BannerRoutine());
        }

        private IEnumerator BannerRoutine()
        {
            Vector2 basePos = _rt.anchoredPosition;
            Vector2 from = basePos + Vector2.left * _travelDistance;
            Vector2 to = basePos + Vector2.right * _travelDistance;

            // Slide in
            float t = 0f;
            while (t < _slideInDuration)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / _slideInDuration);
                float eased = 1f - Mathf.Pow(1f - u, 3f); // ease-out cubic
                _rt.anchoredPosition = Vector2.Lerp(from, basePos, eased);
                _group.alpha = eased;
                yield return null;
            }
            _rt.anchoredPosition = basePos;
            _group.alpha = 1f;

            // Hold
            float hold = 0f;
            while (hold < _holdDuration)
            {
                hold += Time.unscaledDeltaTime;
                yield return null;
            }

            // Slide out
            t = 0f;
            while (t < _slideOutDuration)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / _slideOutDuration);
                float eased = u * u; // ease-in
                _rt.anchoredPosition = Vector2.Lerp(basePos, to, eased);
                _group.alpha = 1f - u;
                yield return null;
            }

            _rt.anchoredPosition = basePos;
            _group.alpha = 0f;
            _routine = null;
        }
    }
}
