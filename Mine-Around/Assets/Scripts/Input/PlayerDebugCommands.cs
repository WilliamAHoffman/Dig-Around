using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerDebugCommands : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private InputActionReference toggleCollision;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Collider2D playerCollider;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
        }
    }

    void Start()
    {
        #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            Destroy(GameObject);
        #endif
    }

    private void OnEnable()
    {
        teleportAction.action.performed += OnTeleport;
        teleportAction.action.Enable();

        toggleCollision.action.performed += ToggleIntangible;
        toggleCollision.action.Enable();
    }

    private void OnDisable()
    {
        teleportAction.action.performed -= OnTeleport;
        teleportAction.action.Disable();

        toggleCollision.action.performed -= ToggleIntangible;
        toggleCollision.action.Disable();
    }

    public void SetIntangible(bool isIntangible)
    {
        playerCollider.enabled = !isIntangible;
    }

    public void ToggleIntangible(InputAction.CallbackContext context)
    {
        playerCollider.enabled = !playerCollider.enabled;
        Debug.Log(playerCollider + " switched to: " + playerCollider.enabled);
    }

    public void TeleportTo(Vector2 worldPosition)
    {
        playerMovement.Teleport(worldPosition);
    }

    private void OnTeleport(InputAction.CallbackContext context)
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = playerCamera.ScreenToWorldPoint(screenPosition);

        TeleportTo(new Vector2(worldPosition.x, worldPosition.y));
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (teleportAction == null)
        {
            Debug.LogError("Teleport action is not assigned.", this);
            isValid = false;
        }

        if (toggleCollision == null)
        {
            Debug.LogError("ToggleCollision is not assigned.", this);
            isValid = false;
        }


        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned.", this);
            isValid = false;
        }

        if (playerMovement == null)
        {
            Debug.LogError("Player movement is not assigned.", this);
            isValid = false;
        }

        if (playerCollider == null)
        {
            Debug.LogError("Player collider is not assigned.", this);
            isValid = false;
        }

        return isValid;
    }
}