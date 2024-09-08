using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody _rb;
    // private Animator _animator;
    
    private PlayerInputActions _playerControls;
    private Vector2 _moveInput;

    private bool _canMove = true;
    private bool _isMoving;

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerControls = new PlayerInputActions();
    }
    
    private void OnEnable()
    {
        _playerControls.Player.Move.Enable();
        _playerControls.Player.Move.performed += OnMove;
        _playerControls.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        _playerControls.Player.Move.Disable();
        _playerControls.Player.Move.performed -= OnMove;
        _playerControls.Player.Move.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        SetMoveInput(context.ReadValue<Vector2>());
    }

    protected void SetMoveInput(Vector2 newMoveInput)
    {
        _moveInput = newMoveInput;
    }

    private void FixedUpdate()
    {
        var hasMoved = TryMoving();
        if (_isMoving != hasMoved)
        {
            _isMoving = hasMoved;
            // _animator.SetBool(AnimatorParameters.IsWalking, hasMoved);
        }
    }

    private bool TryMoving()
    {
        if (!_canMove)
            return false;

        var moveVector = new Vector3(_moveInput.x, 0, _moveInput.y) * (moveSpeed * Time.fixedDeltaTime);
        if (moveVector == Vector3.zero)
            return false;

        _rb.MovePosition(_rb.position + moveVector);
        var toRotation = Quaternion.LookRotation(moveVector, Vector3.up);
        _rb.rotation = Quaternion.RotateTowards(_rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        return true;
    }
}
