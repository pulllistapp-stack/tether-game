using UnityEngine;
using UnityEngine.InputSystem;

namespace Tether.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField, Range(0f, 1f)] private float _airControl = 0.7f;
        [SerializeField] private float _accelTime = 0.08f;
        [SerializeField] private float _decelTime = 0.05f;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 3f;
        [SerializeField] private float _jumpTimeToApex = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _variableJumpCut = 0.5f;
        [SerializeField] private float _coyoteTime = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.15f;
        [SerializeField] private float _maxFallSpeed = 18f;
        [SerializeField] private float _fallGravityMultiplier = 1.5f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Transform _groundCheckPoint;
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(1.3f, 0.12f);

        [Header("Animator")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private static readonly int H_Speed = Animator.StringToHash("Speed");
        private static readonly int H_Grounded = Animator.StringToHash("IsGrounded");
        private static readonly int H_YVel = Animator.StringToHash("YVelocity");
        private static readonly int H_Attack = Animator.StringToHash("AttackTrigger");

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _isGrounded;
        private float _gravityScale;
        private float _jumpVelocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;
            if (_animator == null) _animator = GetComponent<Animator>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            RecalcJumpPhysics();
        }

        private void OnValidate() { RecalcJumpPhysics(); }

        private void RecalcJumpPhysics()
        {
            float t = Mathf.Max(_jumpTimeToApex, 0.01f);
            float gravity = (2f * _jumpHeight) / (t * t);
            _jumpVelocity = gravity * t;
            _gravityScale = gravity / Mathf.Abs(Physics2D.gravity.y);
        }

        private void Update()
        {
            _coyoteTimer -= Time.deltaTime;
            _jumpBufferTimer -= Time.deltaTime;
            if (_isGrounded) _coyoteTimer = _coyoteTime;
            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f) DoJump();

            if (_animator != null)
            {
                _animator.SetFloat(H_Speed, Mathf.Abs(_rb.linearVelocity.x));
                _animator.SetBool(H_Grounded, _isGrounded);
                _animator.SetFloat(H_YVel, _rb.linearVelocity.y);
            }
        }

        private void FixedUpdate()
        {
            _isGrounded = _groundCheckPoint != null && Physics2D.OverlapBox(
                _groundCheckPoint.position, _groundCheckSize, 0f, _groundLayer);

            float targetVelX = _moveInput.x * _moveSpeed;
            float airFactor = _isGrounded ? 1f : _airControl;
            bool isAccelerating = Mathf.Abs(targetVelX) > 0.01f;
            float t = isAccelerating ? _accelTime : _decelTime;
            float maxDelta = (_moveSpeed / Mathf.Max(t, 0.001f)) * airFactor * Time.fixedDeltaTime;
            float newVelX = Mathf.MoveTowards(_rb.linearVelocity.x, targetVelX, maxDelta);

            _rb.gravityScale = _rb.linearVelocity.y < 0f ? _gravityScale * _fallGravityMultiplier : _gravityScale;
            float newVelY = Mathf.Max(_rb.linearVelocity.y, -_maxFallSpeed);
            _rb.linearVelocity = new Vector2(newVelX, newVelY);
        }

        private void DoJump()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpVelocity);
            _coyoteTimer = 0f;
            _jumpBufferTimer = 0f;
        }

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            if (_spriteRenderer != null && Mathf.Abs(_moveInput.x) > 0.1f)
            {
                _spriteRenderer.flipX = _moveInput.x < 0f;
            }
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed) _jumpBufferTimer = _jumpBuffer;
            else if (_rb.linearVelocity.y > 0f)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * _variableJumpCut);
            }
        }

        public void TriggerAttackAnim()
        {
            if (_animator != null) _animator.SetTrigger(H_Attack);
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheckPoint == null) return;
            Gizmos.color = Application.isPlaying && _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        }
    }
}
