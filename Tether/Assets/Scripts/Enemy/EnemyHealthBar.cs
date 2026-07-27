using UnityEngine;

namespace Tether.Enemy
{
    /// <summary>
    /// Phase 8 enemy health bar. Two sprite quads (track + fill) parented above
    /// the enemy. Hidden until the enemy takes its first hit, then fades out
    /// after a idle delay so untouched waves stay visually clean.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private float _width = 1.25f;
        [SerializeField] private float _height = 0.2f;
        [Tooltip("World-space gap above the sprite's top edge — the bar auto-clears taller enemies.")]
        [SerializeField] private float _yPadding = 0.28f;
        [SerializeField] private float _visibleDuration = 2.5f;
        [SerializeField] private float _fadeDuration = 0.35f;

        private SpriteRenderer _track;
        private SpriteRenderer _fill;
        private float _lastDamageAt = -999f;
        private bool _everDamaged;
        private float _ratio = 1f;

        /// <summary>Builds the bar. Sprite comes from the enemy's own renderer so we
        /// don't need a dedicated asset — a square-ish sprite scaled thin reads fine.</summary>
        public void Build(Sprite barSprite, int baseSortingOrder)
        {
            if (barSprite == null) return;

            // Bar sits in world space but must not inherit the enemy's size multiplier,
            // otherwise a 3x boss gets a 3x bar. Compensate by dividing by lossyScale.
            var trackGO = new GameObject("HpBarTrack");
            trackGO.transform.SetParent(transform, false);
            _track = trackGO.AddComponent<SpriteRenderer>();
            _track.sprite = barSprite;
            _track.color = new Color(0.05f, 0.05f, 0.08f, 0f);
            _track.sortingOrder = baseSortingOrder + 4;

            var fillGO = new GameObject("HpBarFill");
            fillGO.transform.SetParent(transform, false);
            _fill = fillGO.AddComponent<SpriteRenderer>();
            _fill.sprite = barSprite;
            _fill.color = new Color(0.9f, 0.25f, 0.3f, 0f);
            _fill.sortingOrder = baseSortingOrder + 5;

            LayoutBar();
        }

        private void LayoutBar()
        {
            if (_track == null || _fill == null) return;

            // Undo parent scaling so every enemy shows the same bar size on screen
            float parentScale = Mathf.Max(0.01f, transform.lossyScale.x);
            float spriteW = _track.sprite.bounds.size.x;
            float spriteH = _track.sprite.bounds.size.y;

            float sx = (_width / parentScale) / Mathf.Max(0.001f, spriteW);
            float sy = (_height / parentScale) / Mathf.Max(0.001f, spriteH);

            // Sit just above the enemy's own sprite instead of a fixed offset, so a
            // 2.5x boss doesn't wear its bar through its chest.
            var ownRenderer = GetComponent<SpriteRenderer>();
            float spriteTopWorld = ownRenderer != null && ownRenderer.sprite != null
                ? ownRenderer.sprite.bounds.extents.y * parentScale
                : 0.5f;
            float localY = (spriteTopWorld + _yPadding) / parentScale;

            _track.transform.localScale = new Vector3(sx, sy, 1f);
            _track.transform.localPosition = new Vector3(0f, localY, 0f);

            // Fill anchors from the left edge so it drains rightward
            _fill.transform.localScale = new Vector3(sx * _ratio, sy, 1f);
            float leftEdge = -(_width / parentScale) * 0.5f;
            _fill.transform.localPosition = new Vector3(
                leftEdge + (_width / parentScale) * _ratio * 0.5f, localY, 0f);
        }

        /// <summary>Called by Enemy whenever HP changes.</summary>
        public void SetRatio(float ratio01)
        {
            _ratio = Mathf.Clamp01(ratio01);
            _everDamaged = true;
            _lastDamageAt = Time.time;
            LayoutBar();

            // Colour shifts red → orange as HP drops so low-HP targets pop
            if (_fill != null)
            {
                var c = Color.Lerp(new Color(1f, 0.5f, 0.2f), new Color(0.35f, 0.9f, 0.4f), _ratio);
                _fill.color = new Color(c.r, c.g, c.b, _fill.color.a);
            }
        }

        private void LateUpdate()
        {
            if (_track == null || _fill == null) return;

            float targetAlpha = 0f;
            if (_everDamaged)
            {
                float since = Time.time - _lastDamageAt;
                if (since < _visibleDuration) targetAlpha = 1f;
                else targetAlpha = Mathf.Clamp01(1f - (since - _visibleDuration) / _fadeDuration);
            }

            SetAlpha(_track, targetAlpha * 0.75f);
            SetAlpha(_fill, targetAlpha);
        }

        private static void SetAlpha(SpriteRenderer sr, float a)
        {
            var c = sr.color;
            if (Mathf.Approximately(c.a, a)) return;
            c.a = a;
            sr.color = c;
        }
    }
}
