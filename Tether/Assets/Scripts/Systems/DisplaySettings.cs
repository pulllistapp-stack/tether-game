using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 8 display tuning. Persistent singleton that letterboxes the main
    /// camera to a chosen aspect and can push a resolution to the window.
    ///
    /// Aspect is applied via the camera's viewport rect rather than the window
    /// size, because Screen.SetResolution is a no-op inside the editor — this
    /// way the framing preview works while play-testing, not just in builds.
    /// </summary>
    public class DisplaySettings : MonoBehaviour
    {
        public static DisplaySettings Instance { get; private set; }

        public struct AspectOption
        {
            public string label;
            public float ratio;   // width / height; <= 0 means "fill the window"
            public AspectOption(string l, float r) { label = l; ratio = r; }
        }

        public struct ResolutionOption
        {
            public string label;
            public int width;
            public int height;
            public ResolutionOption(string l, int w, int h) { label = l; width = w; height = h; }
        }

        public static readonly AspectOption[] Aspects =
        {
            new AspectOption("Free (fill)", 0f),
            new AspectOption("16:9",   16f / 9f),
            new AspectOption("16:10",  16f / 10f),
            new AspectOption("3:2",    3f / 2f),
            new AspectOption("4:3",    4f / 3f),
            new AspectOption("21:9",   21f / 9f),
            new AspectOption("1:1",    1f),
            new AspectOption("4:5",    4f / 5f),
            new AspectOption("3:4",    3f / 4f),
            new AspectOption("9:16",   9f / 16f),
        };

        public static readonly ResolutionOption[] Resolutions =
        {
            new ResolutionOption("1280 x 720",   1280, 720),
            new ResolutionOption("1600 x 900",   1600, 900),
            new ResolutionOption("1920 x 1080",  1920, 1080),
            new ResolutionOption("2560 x 1440",  2560, 1440),
            new ResolutionOption("1080 x 1920",  1080, 1920),
            new ResolutionOption("1440 x 1080",  1440, 1080),
        };

        private const string PrefAspect = "tether.display.aspect";
        private const string PrefResolution = "tether.display.resolution";
        private const string PrefFullscreen = "tether.display.fullscreen";
        private const string PrefFitMode = "tether.display.fitmode";
        private const string PrefZoom = "tether.display.zoom";

        public int AspectIndex { get; private set; }
        public int ResolutionIndex { get; private set; } = 2; // 1920x1080
        public bool Fullscreen { get; private set; }
        public CameraFitter.FitMode FitMode { get; private set; } = CameraFitter.FitMode.FitAll;
        public float Zoom { get; private set; } = 1.04f;

        /// <summary>Arena half-extents the camera frames against. Kept here so the
        /// fitter attached to each scene's camera gets the same numbers.</summary>
        [SerializeField] private float _arenaHalfWidth = 6.5f;
        [SerializeField] private float _arenaHalfHeight = 10f;

        private Camera _cachedCamera;
        private CameraFitter _cachedFitter;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AspectIndex = Mathf.Clamp(PlayerPrefs.GetInt(PrefAspect, 0), 0, Aspects.Length - 1);
            ResolutionIndex = Mathf.Clamp(PlayerPrefs.GetInt(PrefResolution, 2), 0, Resolutions.Length - 1);
            Fullscreen = PlayerPrefs.GetInt(PrefFullscreen, 0) == 1;
            FitMode = (CameraFitter.FitMode)Mathf.Clamp(PlayerPrefs.GetInt(PrefFitMode, 0), 0, 2);
            Zoom = Mathf.Clamp(PlayerPrefs.GetFloat(PrefZoom, 1.04f), 0.6f, 2f);

            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start() { ApplyAspect(); EnsureFitter(); }

        private void HandleSceneLoaded(Scene s, LoadSceneMode m)
        {
            // Each scene brings its own camera
            _cachedCamera = null;
            _cachedFitter = null;
            ApplyAspect();
            EnsureFitter();
        }

        private void LateUpdate()
        {
            // The window can be resized at any time, so keep the letterbox honest.
            ApplyAspect();
            if (_cachedFitter == null) EnsureFitter();
        }

        /// <summary>Attaches a CameraFitter to the active camera and pushes current framing.</summary>
        private void EnsureFitter()
        {
            var cam = Camera.main;
            if (cam == null) return;

            _cachedFitter = cam.GetComponent<CameraFitter>();
            if (_cachedFitter == null) _cachedFitter = cam.gameObject.AddComponent<CameraFitter>();

            _cachedFitter.SetArenaBounds(_arenaHalfWidth, _arenaHalfHeight);
            _cachedFitter.SetMode(FitMode);
            _cachedFitter.SetPadding(Zoom);
        }

        public void SetFitMode(CameraFitter.FitMode mode)
        {
            FitMode = mode;
            PlayerPrefs.SetInt(PrefFitMode, (int)mode);
            PlayerPrefs.Save();
            if (_cachedFitter != null) _cachedFitter.SetMode(mode);
        }

        public void SetZoom(float zoom)
        {
            Zoom = Mathf.Clamp(zoom, 0.6f, 2f);
            PlayerPrefs.SetFloat(PrefZoom, Zoom);
            PlayerPrefs.Save();
            if (_cachedFitter != null) _cachedFitter.SetPadding(Zoom);
        }

        public void SetAspect(int index)
        {
            AspectIndex = Mathf.Clamp(index, 0, Aspects.Length - 1);
            PlayerPrefs.SetInt(PrefAspect, AspectIndex);
            PlayerPrefs.Save();
            ApplyAspect();
        }

        public void SetResolution(int index)
        {
            ResolutionIndex = Mathf.Clamp(index, 0, Resolutions.Length - 1);
            PlayerPrefs.SetInt(PrefResolution, ResolutionIndex);
            PlayerPrefs.Save();
            ApplyResolution();
        }

        public void SetFullscreen(bool on)
        {
            Fullscreen = on;
            PlayerPrefs.SetInt(PrefFullscreen, on ? 1 : 0);
            PlayerPrefs.Save();
            ApplyResolution();
        }

        public void ApplyResolution()
        {
            var r = Resolutions[ResolutionIndex];
#if UNITY_EDITOR
            // Screen.SetResolution is ignored in the editor — the Game view size
            // owns the window there. Framing still updates because CameraFitter
            // reacts to whatever aspect the Game view is actually at.
            return;
#else
            Screen.SetResolution(r.width, r.height,
                Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
#endif
        }

        public bool IsEditorPreview
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        private void ApplyAspect()
        {
            var cam = _cachedCamera;
            if (cam == null)
            {
                cam = Camera.main;
                _cachedCamera = cam;
            }
            if (cam == null) return;

            float target = Aspects[AspectIndex].ratio;
            if (target <= 0f)
            {
                cam.rect = new Rect(0f, 0f, 1f, 1f);
                return;
            }

            float windowAspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            float scaleHeight = windowAspect / target;

            if (scaleHeight < 1f)
            {
                // Window is taller than target → bars on top and bottom
                cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
            }
            else
            {
                // Window is wider than target → bars on left and right
                float scaleWidth = 1f / scaleHeight;
                cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
            }
        }

        public string CurrentAspectLabel => Aspects[AspectIndex].label;
        public string CurrentResolutionLabel => Resolutions[ResolutionIndex].label;
    }
}
