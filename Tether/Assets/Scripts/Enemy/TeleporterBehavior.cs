using UnityEngine;
using System.Collections;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 6 teleporter layer. Every _interval seconds, blink to a new
    /// random X position at the current Y with a brief fade in/out visual.
    /// </summary>
    public class TeleporterBehavior : MonoBehaviour
    {
        [SerializeField] private float _interval = 2.5f;
        [SerializeField] private float _rangeX = 4f;
        [SerializeField] private float _fadeDuration = 0.15f;

        private SpriteRenderer _sr;
        private Color _baseColor;
        private Coroutine _routine;

        public void Configure(EnemyData data)
        {
            if (data == null) return;
            _interval = data.teleportInterval;
            _rangeX = data.teleportRangeX;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseColor = _sr.color;
        }

        private void Start()
        {
            _routine = StartCoroutine(BlinkLoop());
        }

        private IEnumerator BlinkLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(_interval);
                yield return StartCoroutine(BlinkOnce());
            }
        }

        private IEnumerator BlinkOnce()
        {
            // fade out
            float t = 0f;
            while (t < _fadeDuration)
            {
                t += Time.deltaTime;
                float u = 1f - Mathf.Clamp01(t / _fadeDuration);
                if (_sr != null) _sr.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, u);
                yield return null;
            }

            // teleport
            float newX = Random.Range(-_rangeX, _rangeX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // fade in
            t = 0f;
            while (t < _fadeDuration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / _fadeDuration);
                if (_sr != null) _sr.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, u);
                yield return null;
            }
            if (_sr != null) _sr.color = _baseColor;
        }
    }
}
