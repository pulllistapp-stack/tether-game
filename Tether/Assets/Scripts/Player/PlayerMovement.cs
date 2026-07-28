using UnityEngine;

namespace Tether.Player
{
    /// <summary>
    /// Player movement. WASD / arrow keys → 8-way movement inside the arena.
    /// Uses Rigidbody2D.MovePosition when a Kinematic RB is present, else transform.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 6f;
        [Tooltip("If true, allow vertical movement too; else strafe only.")]
        [SerializeField] private bool _verticalMovement = true;

        [Header("Bounds (world units)")]
        [SerializeField] private Vector2 _boundsMin = new Vector2(-5.5f, -7f);
        [SerializeField] private Vector2 _boundsMax = new Vector2(5.5f, 6.5f);

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = _verticalMovement ? Input.GetAxisRaw("Vertical") : 0f;
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector2 delta = input * _speed * Time.deltaTime;
            Vector2 target = (Vector2)transform.position + delta;
            target.x = Mathf.Clamp(target.x, _boundsMin.x, _boundsMax.x);
            target.y = Mathf.Clamp(target.y, _boundsMin.y, _boundsMax.y);

            if (_rb != null && _rb.bodyType == RigidbodyType2D.Kinematic)
            {
                _rb.MovePosition(target);
            }
            else
            {
                transform.position = new Vector3(target.x, target.y, transform.position.z);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.4f, 1f, 0.5f, 0.4f);
            Vector3 center = new Vector3(
                (_boundsMin.x + _boundsMax.x) * 0.5f,
                (_boundsMin.y + _boundsMax.y) * 0.5f, 0f);
            Vector3 size = new Vector3(
                _boundsMax.x - _boundsMin.x,
                _boundsMax.y - _boundsMin.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
