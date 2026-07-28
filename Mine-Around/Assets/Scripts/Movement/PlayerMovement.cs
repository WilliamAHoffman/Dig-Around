using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public sealed class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float sprintSpeed = 15f;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private bool _isSprinting;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetMovementInput(Vector2 input)
    {
        _movementInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void SetSprinting(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }

    public void ToggleSprinting()
    {
        _isSprinting = !_isSprinting;
    }

    public void Teleport(Vector2 worldPosition)
    {
        _rigidbody.position = worldPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }

    private void Move()
    {
        float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;

        Vector2 nextPosition =
            _rigidbody.position +
            _movementInput * currentSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);
    }
}
