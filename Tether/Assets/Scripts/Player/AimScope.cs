using UnityEngine;

namespace Tether.Player
{
    /// <summary>
    /// Places itself at the mouse cursor's world position each frame and
    /// hides the OS cursor while active. Purely visual — actual aim is
    /// still owned by Player.GetAimDirection().
    /// </summary>
    public class AimScope : MonoBehaviour
    {
        [SerializeField] private float _zDepth = 0f;
        [SerializeField] private bool _hideOsCursor = true;
        [Tooltip("Optional gentle rotation for a lively feel.")]
        [SerializeField] private float _spinSpeed = 12f;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            if (_hideOsCursor) Cursor.visible = false;
        }

        private void OnEnable()
        {
            if (_hideOsCursor) Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera == null) return;

            Vector3 world = _camera.ScreenToWorldPoint(Input.mousePosition);
            world.z = _zDepth;
            transform.position = world;

            if (_spinSpeed != 0f)
                transform.Rotate(0f, 0f, _spinSpeed * Time.unscaledDeltaTime);
        }
    }
}
