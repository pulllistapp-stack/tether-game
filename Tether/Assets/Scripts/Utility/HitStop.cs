using System.Collections;
using UnityEngine;

namespace Tether.Utility
{
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        private Coroutine _current;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Freeze(float duration)
        {
            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            Time.timeScale = 0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            Time.timeScale = 1f;
            _current = null;
        }
    }
}
