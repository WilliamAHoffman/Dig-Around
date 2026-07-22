using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference toggleSprint;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private Camera playerCamera;

    //movement variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] float sprintSpeed = 15f;


    private Rigidbody2D rb;
    private Collider2D cld;
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool sprinting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cld = GetComponent<Collider2D>();
        if(!moveAction || !playerCamera || !clickAction || !toggleSprint)
        {
            Debug.LogError("Missing a reference", this);
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();

        clickAction.action.performed += TeleportToMouse;
        clickAction.action.Enable();

        toggleSprint.action.started += ToggleSprint;
        toggleSprint.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();

        clickAction.action.performed -= TeleportToMouse;
        clickAction.action.Disable();

        toggleSprint.action.started -= ToggleSprint;
        toggleSprint.action.Disable();
    }

    private void Update()
    {
        movement = moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        float speed = moveSpeed;
        if(sprinting) speed = sprintSpeed;

        Vector2 nextPosition = rb.position + movement * speed * Time.fixedDeltaTime;

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

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        Debug.Log("sprinting");
        sprinting = !sprinting;
        cld.enabled = !sprinting;
    }
}