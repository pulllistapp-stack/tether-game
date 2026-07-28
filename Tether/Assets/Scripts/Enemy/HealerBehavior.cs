using UnityEngine;
using System.Collections;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 6 healer layer. Periodically pulses a heal on every enemy
    /// (except self) within _radius. Emits a brief green flash to telegraph.
    /// </summary>
    public class HealerBehavior : MonoBehaviour
    {
        [SerializeField] private float _interval = 3f;
        [SerializeField] private float _radius = 2.5f;
        [SerializeField] private float _healAmount = 2f;

        private Coroutine _routine;
        private SpriteRenderer _sr;
        private Color _baseColor;

        public void Configure(EnemyData data)
        {
            if (data == null) return;
            _interval = data.healInterval;
            _radius = data.healRadius;
            _healAmount = data.healAmount;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseColor = _sr.color;
        }

        private void Start()
        {
            _routine = StartCoroutine(PulseLoop());
        }

        private IEnumerator PulseLoop()
        {
            yield return new WaitForSeconds(1.2f);
            while (true)
            {
                Pulse();
                yield return new WaitForSeconds(_interval);
            }
        }

        private void Pulse()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _radius);
            foreach (var h in hits)
            {
                var e = h.GetComponent<Enemy>();
                if (e == null || e == GetComponent<Enemy>()) continue;
                e.Heal(_healAmount);
            }
            if (_sr != null) StartCoroutine(FlashGreen());
        }

        private IEnumerator FlashGreen()
        {
            var glow = new Color(0.4f, 1f, 0.6f);
            _sr.color = glow;
            yield return new WaitForSeconds(0.15f);
            _sr.color = _baseColor;
        }
    }
}
