using UnityEngine;

namespace Tether.Player
{
    /// <summary>
    /// Places itself at the mouse cursor's world position each frame and
    /// hides the OS cursor while active. Only visible during actual gameplay —
    /// hides itself (and shows the OS cursor) whenever the game is paused
    /// (Time.timeScale == 0) or the run is over.
    /// </summary>
    public class AimScope : MonoBehaviour
    {
        [SerializeField] private float _zDepth = 0f;
        [Tooltip("Optional gentle rotation for a lively feel.")]
        [SerializeField] private float _spinSpeed = 12f;

        private Camera _camera;
        private SpriteRenderer _sr;
        private Systems.RunController _runController;
        private bool _scopeShown;

        private void Awake()
        {
            _camera = Camera.main;
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _runController = FindFirstObjectByType<Systems.RunController>();
        }

        private void OnDisable()
        {
            SetScopeVisible(false);
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            if (_camera == null) _camera = Camera.main;

            bool shouldShow = ShouldScopeBeActive();
            SetScopeVisible(shouldShow);

            if (!shouldShow) return;

            Vector3 world = _camera.ScreenToWorldPoint(Input.mousePosition);
            world.z = _zDepth;
            transform.position = world;

            if (_spinSpeed != 0f)
                transform.Rotate(0f, 0f, _spinSpeed * Time.unscaledDeltaTime);
        }

        private bool ShouldScopeBeActive()
        {
            // Paused (upgrade modal / pause menu) → give the OS cursor back
            if (Time.timeScale <= 0.0001f) return false;

            // Run not in Playing state (game over / victory) → same
            if (_runController != null &&
                _runController.State != Systems.RunController.RunState.Playing)
                return false;

            return true;
        }

        private void SetScopeVisible(bool visible)
        {
            if (visible == _scopeShown) return;
            _scopeShown = visible;
            if (_sr != null) _sr.enabled = visible;
            Cursor.visible = !visible;
        }
    }
}
