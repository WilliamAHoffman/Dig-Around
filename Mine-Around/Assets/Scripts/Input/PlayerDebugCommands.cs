using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class PlayerDebugCommands : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private InputActionReference toggleCollision;
    [SerializeField] private InputActionReference toggleEditor;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private UIDocument worldEditor;

    private bool editorOpened;
    

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

        toggleEditor.action.performed += ToggleEditor;
        toggleEditor.action.Enable();
    }

    private void OnDisable()
    {
        teleportAction.action.performed -= OnTeleport;
        teleportAction.action.Disable();

        toggleCollision.action.performed -= ToggleIntangible;
        toggleCollision.action.Disable();

        toggleEditor.action.performed -= ToggleEditor;
        toggleEditor.action.Disable();
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

    public void ToggleEditor(InputAction.CallbackContext context)
    {
        editorOpened = !editorOpened;
        if (editorOpened)
        {
            worldEditor.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            worldEditor.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    public void TeleportTo(Vector2 worldPosition)
    {
        playerMovement.Teleport(worldPosition);
    }

    private void OnTeleport(InputAction.CallbackContext context)
    {
        if(editorOpened) return;
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