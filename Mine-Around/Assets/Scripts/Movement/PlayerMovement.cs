using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private Camera playerCamera;
    private Rigidbody2D rb;
    [SerializeField] private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(!moveAction || !playerCamera)
        {
            Debug.LogError("Missing a reference", this);
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();

        clickAction.action.performed += TeleportToMouse;
        clickAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();

        clickAction.action.performed -= TeleportToMouse;
        clickAction.action.Disable();
    }

    private void Update()
    {
        movement = moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition =
            rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void TeleportToMouse(InputAction.CallbackContext context)
    {
        if (playerCamera == null || Mouse.current == null)
            return;

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition =
            playerCamera.ScreenToWorldPoint(screenPosition);

        rb.position = new Vector2(
            worldPosition.x,
            worldPosition.y
        );
    }
}