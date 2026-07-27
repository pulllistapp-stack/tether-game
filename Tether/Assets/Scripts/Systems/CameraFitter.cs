using UnityEngine;

namespace Tether.Systems
{
    /// <summary>
    /// Phase 8 camera framing. Computes orthographicSize so the arena actually
    /// fills the viewport at whatever aspect the window happens to be, instead
    /// of leaving a fixed size that wastes half the screen on wide monitors.
    ///
    /// Fit rule: pick whichever axis is the binding constraint. A wide window is
    /// limited by arena height; a tall window is limited by arena width.
    /// </summary>
    [DefaultExecutionOrder(200)] // after DisplaySettings has set the camera rect
    public class CameraFitter : MonoBehaviour
    {
        public enum FitMode
        {
            FitAll,      // whole arena visible, letterboxed by whichever axis binds
            FillWidth,   // arena width spans the viewport; top/bottom may overflow
            FillHeight,  // arena height spans the viewport; sides may overflow
        }

        [Header("Arena bounds (world units, half-extents)")]
        [SerializeField] private float _arenaHalfWidth = 9.5f;
        [SerializeField] private float _arenaHalfHeight = 8f;

        [Header("Framing")]
        [SerializeField] private FitMode _mode = FitMode.FitAll;
        [Tooltip("1 = arena exactly touches the viewport edge. >1 leaves margin, <1 crops in.")]
        [SerializeField] private float _padding = 1.04f;

        private Camera _camera;
        private float _lastAspect = -1f;
        private FitMode _lastMode;
        private float _lastPadding = -1f;

        public FitMode Mode => _mode;
        public float Padding => _padding;

        public void SetMode(FitMode mode) { _mode = mode; Invalidate(); }
        public void SetPadding(float padding) { _padding = Mathf.Max(0.5f, padding); Invalidate(); }
        public void SetArenaBounds(float halfWidth, float halfHeight)
        {
            _arenaHalfWidth = halfWidth;
            _arenaHalfHeight = halfHeight;
            Invalidate();
        }

        private void Invalidate() => _lastAspect = -1f;

        private void Awake() => _camera = GetComponent<Camera>();

        private void LateUpdate()
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null || !_camera.orthographic) return;

            // camera.aspect already accounts for a letterboxed viewport rect
            float aspect = _camera.aspect;
            if (Mathf.Approximately(aspect, _lastAspect) &&
                _mode == _lastMode &&
                Mathf.Approximately(_padding, _lastPadding)) return;

            _lastAspect = aspect;
            _lastMode = _mode;
            _lastPadding = _padding;

            // orthographicSize is the half-height of the view.
            // Half-width = size * aspect, so fitting width needs size = halfWidth / aspect.
            float sizeForHeight = _arenaHalfHeight;
            float sizeForWidth = _arenaHalfWidth / Mathf.Max(0.0001f, aspect);

            float size;
            switch (_mode)
            {
                case FitMode.FillWidth:  size = sizeForWidth; break;
                case FitMode.FillHeight: size = sizeForHeight; break;
                default:                 size = Mathf.Max(sizeForHeight, sizeForWidth); break;
            }

            _camera.orthographicSize = size * _padding;
        }
    }
}
