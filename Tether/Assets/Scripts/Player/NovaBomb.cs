using UnityEngine;

namespace Tether.Player
{
    /// <summary>
    /// Phase 4 active skill. Press E to detonate an area-of-effect burst
    /// centered on the player: damages all enemies in radius. On cooldown.
    /// </summary>
    public class NovaBomb : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private KeyCode _key = KeyCode.E;
        [SerializeField] private float _cooldown = 10f;

        [Header("Blast")]
        [SerializeField] private float _radius = 4.5f;
        [SerializeField] private float _damage = 6f;
        [SerializeField] private LayerMask _enemyLayers = ~0;

        [Header("Feel")]
        [SerializeField] private float _shake = 0.6f;
        [SerializeField] private float _hitStop = 0.06f;
        [SerializeField] private Color _flashColor = new Color(0.6f, 0.85f, 1f, 0.7f);

        public float Cooldown => _cooldown;
        public float TimeSinceLastFire => Time.time - _lastFireAt;
        public bool IsReady => TimeSinceLastFire >= _cooldown;

        private float _lastFireAt = -999f;

        private void Update()
        {
            if (Input.GetKeyDown(_key) && IsReady) Detonate();
        }

        private void Detonate()
        {
            _lastFireAt = Time.time;

            var hits = Physics2D.OverlapCircleAll(transform.position, _radius, _enemyLayers);
            foreach (var h in hits)
            {
                if (h.TryGetComponent<Enemy.Enemy>(out var e))
                    e.TakeDamage(_damage);
            }

            if (CameraSystems.CameraShaker.Instance != null)
                CameraSystems.CameraShaker.Instance.Shake(_shake);
            if (Utility.HitStop.Instance != null)
                Utility.HitStop.Instance.Freeze(_hitStop);
            Audio.AudioManager.Instance?.Play("enemy_die", 1.2f, 0.02f); // reuse sweep for now

            SpawnBlastVisual();
        }

        private void SpawnBlastVisual()
        {
            var go = new GameObject("NovaBlast");
            go.transform.position = transform.position;
            var sr = go.AddComponent<SpriteRenderer>();
            // Reuse Circle_16 sprite if we can find it via Resources? Not registered.
            // Fall back to any sprite renderer's sprite via search
            var anyBall = FindFirstObjectByType<Gameplay.Ball>();
            if (anyBall != null)
            {
                var ballSr = anyBall.GetComponent<SpriteRenderer>();
                if (ballSr != null) sr.sprite = ballSr.sprite;
            }
            sr.color = _flashColor;
            sr.sortingOrder = 20;
            go.transform.localScale = Vector3.one * (_radius * 2f);
            go.AddComponent<Utility.FadeAndDestroy>();
        }
    }
}
