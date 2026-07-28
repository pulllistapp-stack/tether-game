using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Tether.Systems
{
    /// <summary>
    /// Builds the scene's 2D lighting and post-processing stack at runtime.
    ///
    /// Doing this in code rather than by hand-placing objects keeps every scene
    /// consistent and means new scenes inherit the look for free. Follows the
    /// Unity 2D art guide: a Global Light for base ambience (one per blend style
    /// per sorting layer), then additive point lights for anything that should
    /// read as a light source.
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class SceneLightingBootstrap : MonoBehaviour
    {
        [Header("Global light")]
        [Tooltip("Base ambience. 1 = fully lit (no darkening); lower values sink the arena into shadow so point lights read.")]
        [Range(0f, 1f)]
        [SerializeField] private float _globalIntensity = 1.0f;
        [SerializeField] private Color _globalColor = Color.white;

        [Header("Post-processing")]
        // Disabled by user request — the only lights on screen should be the ball
        // point-lights attached via AutoLight2D. No bloom, no vignette, no color grade.
        [SerializeField] private bool _enablePostProcessing = false;
        [SerializeField] private float _bloomIntensity = 0f;
        [SerializeField] private float _bloomThreshold = 1f;
        [SerializeField] private float _baseVignette = 0f;
        [SerializeField] private float _saturation = 0f;
        [SerializeField] private float _contrast = 0f;

        public static SceneLightingBootstrap Instance { get; private set; }

        private VolumeProfile _profile;
        private Vignette _vignette;
        private ChromaticAberration _chromatic;
        private Bloom _bloom;

        public float BaseVignette => _baseVignette;
        public float GlobalIntensity => _globalIntensity;
        public float BaseBloomIntensity => _bloomIntensity;

        private void Awake()
        {
            Instance = this;
            EnsureGlobalLight();
            if (_enablePostProcessing) EnsureVolume();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            // The profile is created at runtime, so it must be cleaned up explicitly
            if (_profile != null) Destroy(_profile);
        }

        // ---------------- Lighting ----------------

        private void EnsureGlobalLight()
        {
            // Only one global light may exist per blend style per sorting layer —
            // a second one silently overrides the first, so bail if the scene has one.
            foreach (var existing in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
            {
                if (existing.lightType == Light2D.LightType.Global) return;
            }

            var go = new GameObject("Global Light 2D");
            go.transform.SetParent(transform, false);
            var light = go.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.color = _globalColor;
            light.intensity = _globalIntensity;
        }

        /// <summary>Temporarily drive the global light. Does not change the authored
        /// baseline, so callers can restore with GlobalIntensity.</summary>
        public void SetGlobalIntensity(float value)
        {
            float v = Mathf.Clamp01(value);
            foreach (var l in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
            {
                if (l.lightType == Light2D.LightType.Global) l.intensity = v;
            }
        }

        // ---------------- Post-processing ----------------

        private void EnsureVolume()
        {
            var volumeGO = new GameObject("Global Volume");
            volumeGO.transform.SetParent(transform, false);
            var volume = volumeGO.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0f;

            _profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.sharedProfile = _profile;

            // Bloom — makes balls, XP orbs and the time-stop flash actually glow.
            _bloom = _profile.Add<Bloom>(true);
            _bloom.intensity.Override(_bloomIntensity);
            _bloom.threshold.Override(_bloomThreshold);
            _bloom.scatter.Override(0.68f);
            _bloom.tint.Override(new Color(0.85f, 0.9f, 1f));
            // Guide's optimisation note: high quality filtering is the expensive part
            _bloom.highQualityFiltering.Override(false);

            // Vignette — pulls the eye to the play column and darkens the side margins.
            _vignette = _profile.Add<Vignette>(true);
            _vignette.intensity.Override(_baseVignette);
            _vignette.smoothness.Override(0.45f);
            _vignette.color.Override(new Color(0.02f, 0.01f, 0.06f));

            // Colour grading — cool Scarlet-Devil-Mansion cast.
            var colorAdjust = _profile.Add<ColorAdjustments>(true);
            colorAdjust.saturation.Override(_saturation);
            colorAdjust.contrast.Override(_contrast);
            colorAdjust.postExposure.Override(0.12f);

            // Split toning — blue shadows, warm highlights. Cheap and does a lot.
            var splitToning = _profile.Add<SplitToning>(true);
            splitToning.shadows.Override(new Color(0.25f, 0.35f, 0.7f));
            splitToning.highlights.Override(new Color(1f, 0.86f, 0.62f));
            splitToning.balance.Override(-12f);

            // Chromatic aberration sits at 0 and is pulsed on hit.
            _chromatic = _profile.Add<ChromaticAberration>(true);
            _chromatic.intensity.Override(0f);

            EnsureCameraPostProcessing();
        }

        private void EnsureCameraPostProcessing()
        {
            var cam = Camera.main;
            if (cam == null) return;
            var data = cam.GetUniversalAdditionalCameraData();
            if (data != null) data.renderPostProcessing = true;
        }

        /// <summary>Drives the vignette from outside — used to darken the frame as HP drops.</summary>
        public void SetVignette(float intensity)
        {
            if (_vignette != null) _vignette.intensity.Override(Mathf.Clamp01(intensity));
        }

        /// <summary>Momentary lens distortion, e.g. when the player takes a hit.</summary>
        public void SetChromatic(float intensity)
        {
            if (_chromatic != null) _chromatic.intensity.Override(Mathf.Clamp01(intensity));
        }

        public void SetBloomIntensity(float intensity)
        {
            if (_bloom != null) _bloom.intensity.Override(Mathf.Max(0f, intensity));
        }
    }
}
