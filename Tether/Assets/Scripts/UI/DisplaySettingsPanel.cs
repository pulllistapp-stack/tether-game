using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tether.UI
{
    /// <summary>
    /// Phase 8 display tuning overlay. Press F2 in any scene to cycle aspect
    /// ratios and resolutions live. Builds its own canvas at runtime so it needs
    /// no per-scene wiring, and rides on the DisplaySettings singleton so the
    /// choice survives scene loads.
    ///
    /// This is a tuning tool: pick the framing that feels right, then we bake
    /// it in as the default and this can become a proper options menu.
    /// </summary>
    public class DisplaySettingsPanel : MonoBehaviour
    {
        [SerializeField] private KeyCode _toggleKey = KeyCode.F2;

        private Canvas _canvas;
        private CanvasGroup _group;
        private Font _font;
        private bool _visible;

        private Text _currentLabel;
        private readonly List<Button> _aspectButtons = new List<Button>();
        private readonly List<Button> _resButtons = new List<Button>();
        private Text _fullscreenLabel;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            BuildUi();
            SetVisible(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey)) SetVisible(!_visible);
            if (_visible && Input.GetKeyDown(KeyCode.Escape)) SetVisible(false);
        }

        private void SetVisible(bool on)
        {
            _visible = on;
            if (_group == null) return;
            _group.alpha = on ? 1f : 0f;
            _group.interactable = on;
            _group.blocksRaycasts = on;
            if (on)
            {
                Cursor.visible = true;
                RefreshLabels();
            }
        }

        // ---------------- UI construction ----------------

        private void BuildUi()
        {
            var canvasGO = new GameObject("DisplaySettingsCanvas");
            canvasGO.transform.SetParent(transform, false);
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 500; // above every gameplay canvas
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960f, 540f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            _group = canvasGO.AddComponent<CanvasGroup>();

            // Dim backdrop
            var bg = NewRect("Backdrop", canvasGO.transform);
            bg.anchorMin = Vector2.zero; bg.anchorMax = Vector2.one;
            bg.offsetMin = Vector2.zero; bg.offsetMax = Vector2.zero;
            var bgImg = bg.gameObject.AddComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, 0.82f);

            // Panel
            var panel = NewRect("Panel", canvasGO.transform);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            panel.sizeDelta = new Vector2(660f, 470f);
            var panelImg = panel.gameObject.AddComponent<Image>();
            panelImg.color = new Color(0.09f, 0.1f, 0.15f, 0.98f);

            MakeLabel(panel, "DISPLAY", new Vector2(0f, -22f), new Vector2(620f, 40f),
                26, new Color(1f, 0.95f, 0.85f), TextAnchor.MiddleCenter, FontStyle.Bold,
                new Vector2(0.5f, 1f));

            _currentLabel = MakeLabel(panel, "", new Vector2(0f, -58f), new Vector2(620f, 26f),
                15, new Color(0.65f, 0.95f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal,
                new Vector2(0.5f, 1f));

            // --- Aspect section ---
            MakeLabel(panel, "ASPECT", new Vector2(24f, -96f), new Vector2(200f, 24f),
                15, new Color(0.7f, 0.75f, 0.85f), TextAnchor.MiddleLeft, FontStyle.Bold,
                new Vector2(0f, 1f), new Vector2(0f, 1f));

            var aspects = Systems.DisplaySettings.Aspects;
            BuildGrid(panel, aspects.Length, 5, new Vector2(24f, -124f), 120f, 34f, 8f,
                (i, rt) =>
                {
                    var btn = MakeButton(rt, aspects[i].label, 15);
                    int idx = i;
                    btn.onClick.AddListener(() =>
                    {
                        Systems.DisplaySettings.Instance?.SetAspect(idx);
                        RefreshLabels();
                    });
                    _aspectButtons.Add(btn);
                });

            // --- Resolution section ---
            MakeLabel(panel, "RESOLUTION", new Vector2(24f, -232f), new Vector2(240f, 24f),
                15, new Color(0.7f, 0.75f, 0.85f), TextAnchor.MiddleLeft, FontStyle.Bold,
                new Vector2(0f, 1f), new Vector2(0f, 1f));

            var resolutions = Systems.DisplaySettings.Resolutions;
            BuildGrid(panel, resolutions.Length, 3, new Vector2(24f, -260f), 200f, 34f, 8f,
                (i, rt) =>
                {
                    var btn = MakeButton(rt, resolutions[i].label, 15);
                    int idx = i;
                    btn.onClick.AddListener(() =>
                    {
                        Systems.DisplaySettings.Instance?.SetResolution(idx);
                        RefreshLabels();
                    });
                    _resButtons.Add(btn);
                });

            // --- Fullscreen toggle ---
            var fsRect = NewRect("FullscreenBtn", panel);
            fsRect.anchorMin = new Vector2(0f, 1f); fsRect.anchorMax = new Vector2(0f, 1f);
            fsRect.pivot = new Vector2(0f, 1f);
            fsRect.anchoredPosition = new Vector2(24f, -352f);
            fsRect.sizeDelta = new Vector2(260f, 34f);
            var fsBtn = MakeButton(fsRect, "FULLSCREEN: OFF", 15);
            _fullscreenLabel = fsBtn.GetComponentInChildren<Text>();
            fsBtn.onClick.AddListener(() =>
            {
                var ds = Systems.DisplaySettings.Instance;
                if (ds == null) return;
                ds.SetFullscreen(!ds.Fullscreen);
                RefreshLabels();
            });

            MakeLabel(panel, "F2 or ESC to close   •   editor previews aspect only",
                new Vector2(0f, 20f), new Vector2(620f, 24f),
                13, new Color(0.55f, 0.6f, 0.7f), TextAnchor.MiddleCenter, FontStyle.Normal,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        }

        /// <summary>Lays `count` cells into rows of `perRow` under `parent`.</summary>
        private void BuildGrid(RectTransform parent, int count, int perRow,
                               Vector2 origin, float cellW, float cellH, float gap,
                               System.Action<int, RectTransform> build)
        {
            for (int i = 0; i < count; i++)
            {
                int row = i / perRow;
                int col = i % perRow;
                var rt = NewRect("Cell_" + i, parent);
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(origin.x + col * (cellW + gap),
                                                  origin.y - row * (cellH + gap));
                rt.sizeDelta = new Vector2(cellW, cellH);
                build(i, rt);
            }
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private Text MakeLabel(RectTransform parent, string text, Vector2 pos, Vector2 size,
                               int fontSize, Color color, TextAnchor anchor, FontStyle style,
                               Vector2 pivot, Vector2? anchorMinMax = null)
        {
            var rt = NewRect("Label_" + text, parent);
            Vector2 am = anchorMinMax ?? new Vector2(0.5f, 1f);
            rt.anchorMin = am; rt.anchorMax = am;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var t = rt.gameObject.AddComponent<Text>();
            t.font = _font; t.fontSize = fontSize; t.color = color;
            t.alignment = anchor; t.fontStyle = style;
            t.text = text;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            return t;
        }

        private Button MakeButton(RectTransform rt, string label, int fontSize)
        {
            var img = rt.gameObject.AddComponent<Image>();
            img.color = new Color(0.82f, 0.87f, 1f, 0.9f);
            var btn = rt.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;

            var lrt = NewRect("Label", rt);
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            var t = lrt.gameObject.AddComponent<Text>();
            t.font = _font; t.fontSize = fontSize;
            t.color = new Color(0.05f, 0.05f, 0.1f);
            t.alignment = TextAnchor.MiddleCenter;
            t.fontStyle = FontStyle.Bold;
            t.text = label;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            return btn;
        }

        // ---------------- State reflection ----------------

        private void RefreshLabels()
        {
            var ds = Systems.DisplaySettings.Instance;
            if (ds == null) return;

            if (_currentLabel != null)
                _currentLabel.text = "Aspect  " + ds.CurrentAspectLabel +
                                     "        Resolution  " + ds.CurrentResolutionLabel +
                                     "        Window  " + Screen.width + " x " + Screen.height;

            if (_fullscreenLabel != null)
                _fullscreenLabel.text = "FULLSCREEN: " + (ds.Fullscreen ? "ON" : "OFF");

            // Highlight the active choice so the current pick is obvious
            for (int i = 0; i < _aspectButtons.Count; i++) Tint(_aspectButtons[i], i == ds.AspectIndex);
            for (int i = 0; i < _resButtons.Count; i++) Tint(_resButtons[i], i == ds.ResolutionIndex);
        }

        private static void Tint(Button btn, bool active)
        {
            var img = btn.targetGraphic as Image;
            if (img == null) return;
            img.color = active
                ? new Color(1f, 0.85f, 0.35f, 1f)
                : new Color(0.82f, 0.87f, 1f, 0.9f);
        }
    }
}
