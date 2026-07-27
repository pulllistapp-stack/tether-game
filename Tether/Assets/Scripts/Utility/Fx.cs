using UnityEngine;

namespace Tether.Utility
{
    /// <summary>
    /// Phase 8 FX spawner. Static one-liners so gameplay code stays readable:
    /// callsites say what happened, not how it's drawn. Every effect builds its
    /// own throwaway GameObject and self-destructs — no pooling yet because the
    /// counts here are small (a few per second at peak).
    /// </summary>
    public static class Fx
    {
        private static Sprite _circleSprite;
        private static Sprite _squareSprite;
        private static Font _font;
        private static Material _spriteMat;

        /// <summary>Cached lookups — Resources.Load on every hit would be wasteful.</summary>
        private static Sprite CircleSprite
        {
            get
            {
                if (_circleSprite == null)
                {
                    var ball = Object.FindFirstObjectByType<Gameplay.Ball>();
                    if (ball != null)
                    {
                        var sr = ball.GetComponent<SpriteRenderer>();
                        if (sr != null) _circleSprite = sr.sprite;
                    }
                }
                return _circleSprite;
            }
        }

        private static Font UiFont
        {
            get
            {
                if (_font == null) _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return _font;
            }
        }

        private static Material SpriteMaterial
        {
            get
            {
                if (_spriteMat == null) _spriteMat = new Material(Shader.Find("Sprites/Default"));
                return _spriteMat;
            }
        }

        /// <summary>Sets the sprite used by ring / sparkle effects. Call once at scene setup.</summary>
        public static void SetCircleSprite(Sprite s) => _circleSprite = s;

        // ---------------- Damage numbers ----------------

        /// <summary>Floating damage text that drifts up and fades.</summary>
        public static void DamageNumber(Vector3 worldPos, float amount, bool crit = false)
        {
            var go = new GameObject("DamageNumber");
            go.transform.position = worldPos + new Vector3(Random.Range(-0.15f, 0.15f), 0.25f, 0f);

            var tm = go.AddComponent<TextMesh>();
            tm.font = UiFont;
            tm.GetComponent<MeshRenderer>().sharedMaterial = UiFont.material;
            tm.text = amount >= 10f ? Mathf.RoundToInt(amount).ToString() : amount.ToString("0.#");
            // fontSize drives glyph resolution; characterSize converts to world units.
            // 0.16 puts a digit at roughly 0.5 world units — readable at 12x15 arena scale.
            tm.fontSize = 64;
            tm.characterSize = 0.16f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = crit ? new Color(1f, 0.55f, 0.2f) : new Color(1f, 0.95f, 0.8f);

            var mr = go.GetComponent<MeshRenderer>();
            mr.sortingOrder = 40;

            var mover = go.AddComponent<FloatAndFade>();
            mover.Configure(0.7f, 1.1f, crit ? 1.35f : 1f);
        }

        // ---------------- Rings ----------------

        /// <summary>Expanding ring that fades out — used for wall impacts.</summary>
        public static void ImpactRing(Vector3 worldPos, Color color, float maxRadius = 0.9f, float duration = 0.25f)
        {
            if (CircleSprite == null) return;

            var go = new GameObject("ImpactRing");
            go.transform.position = worldPos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CircleSprite;
            sr.color = color;
            sr.sortingOrder = 12;
            sr.material = SpriteMaterial;

            var ring = go.AddComponent<ExpandAndFade>();
            ring.Configure(0.15f, maxRadius * 2f, duration);
        }

        // ---------------- Sparkles ----------------

        /// <summary>Small burst of shards — used on coin / XP pickup.</summary>
        public static void Sparkle(Vector3 worldPos, Color color, int count = 4, float speed = 3.5f)
        {
            if (CircleSprite == null) return;

            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("Sparkle");
                go.transform.position = worldPos;
                go.transform.localScale = Vector3.one * Random.Range(0.12f, 0.2f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CircleSprite;
                sr.color = color;
                sr.sortingOrder = 14;
                sr.material = SpriteMaterial;

                float angle = (360f / count) * i + Random.Range(-25f, 25f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                var drift = go.AddComponent<DriftAndFade>();
                drift.Configure(dir * Random.Range(speed * 0.6f, speed), 0.35f);
            }
        }
    }

    // ---------------- Behaviours ----------------

    /// <summary>Drifts upward while fading a TextMesh out, with a brief scale punch.</summary>
    public class FloatAndFade : MonoBehaviour
    {
        private float _duration = 0.7f;
        private float _riseDistance = 1.1f;
        private float _scale = 1f;

        private TextMesh _text;
        private Vector3 _start;
        private float _elapsed;
        private Color _baseColor;

        public void Configure(float duration, float riseDistance, float scale)
        {
            _duration = duration;
            _riseDistance = riseDistance;
            _scale = scale;
        }

        private void Awake()
        {
            _text = GetComponent<TextMesh>();
            _start = transform.position;
            if (_text != null) _baseColor = _text.color;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            // Ease-out rise
            float rise = (1f - (1f - t) * (1f - t)) * _riseDistance;
            transform.position = _start + Vector3.up * rise;

            // Punch scale: overshoot early, settle
            float punch = t < 0.2f ? Mathf.Lerp(0.4f, 1.15f, t / 0.2f) : Mathf.Lerp(1.15f, 1f, (t - 0.2f) / 0.8f);
            transform.localScale = Vector3.one * punch * _scale;

            if (_text != null)
            {
                var c = _baseColor;
                c.a = 1f - t * t; // hold visible longer, then drop
                _text.color = c;
            }

            if (t >= 1f) Destroy(gameObject);
        }
    }

    /// <summary>Scales a sprite outward from small to large while fading.</summary>
    public class ExpandAndFade : MonoBehaviour
    {
        private float _startScale = 0.15f;
        private float _endScale = 1.8f;
        private float _duration = 0.25f;

        private SpriteRenderer _sr;
        private float _elapsed;
        private Color _baseColor;

        public void Configure(float startScale, float endScale, float duration)
        {
            _startScale = startScale;
            _endScale = endScale;
            _duration = duration;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseColor = _sr.color;
            transform.localScale = Vector3.one * _startScale;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            float eased = 1f - (1f - t) * (1f - t);

            transform.localScale = Vector3.one * Mathf.Lerp(_startScale, _endScale, eased);

            if (_sr != null)
            {
                var c = _baseColor;
                c.a = _baseColor.a * (1f - t);
                _sr.color = c;
            }

            if (t >= 1f) Destroy(gameObject);
        }
    }

    /// <summary>Drifts in a direction with damping while fading a sprite out.</summary>
    public class DriftAndFade : MonoBehaviour
    {
        private Vector2 _velocity;
        private float _duration = 0.35f;

        private SpriteRenderer _sr;
        private float _elapsed;
        private Color _baseColor;

        public void Configure(Vector2 velocity, float duration)
        {
            _velocity = velocity;
            _duration = duration;
        }

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseColor = _sr.color;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _velocity *= 1f - 4f * Time.deltaTime; // damping

            if (_sr != null)
            {
                var c = _baseColor;
                c.a = _baseColor.a * (1f - t);
                _sr.color = c;
            }
            transform.localScale *= 1f - 1.2f * Time.deltaTime;

            if (t >= 1f) Destroy(gameObject);
        }
    }
}
