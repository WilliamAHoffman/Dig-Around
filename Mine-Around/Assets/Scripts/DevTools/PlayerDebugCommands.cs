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
    [SerializeField] private UIDocument worldEditorList;
    [SerializeField] private WorldEditor worldEditor;

    private bool editorOpened;

    private void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        Destroy(gameObject);
        return;
#endif

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        editorOpened = false;
        worldEditorList.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        teleportAction.action.performed += OnClick;
        teleportAction.action.Enable();

        toggleCollision.action.performed += ToggleIntangible;
        toggleCollision.action.Enable();

        toggleEditor.action.performed += ToggleEditor;
        toggleEditor.action.Enable();
    }

    private void OnDisable()
    {
        if (teleportAction != null)
        {
            teleportAction.action.performed -= OnClick;
            teleportAction.action.Disable();
        }

        if (toggleCollision != null)
        {
            toggleCollision.action.performed -= ToggleIntangible;
            toggleCollision.action.Disable();
        }

        if (toggleEditor != null)
        {
            toggleEditor.action.performed -= ToggleEditor;
            toggleEditor.action.Disable();
        }
    }

    public void SetIntangible(bool isIntangible)
    {
        playerCollider.enabled = !isIntangible;
    }

    private void ToggleIntangible(InputAction.CallbackContext context)
    {
        playerCollider.enabled = !playerCollider.enabled;

        Debug.Log(
            $"Player collider switched to: {playerCollider.enabled}",
            this
        );
    }

    private void ToggleEditor(InputAction.CallbackContext context)
    {
        editorOpened = !editorOpened;

        worldEditorList.rootVisualElement.style.display =
            editorOpened
                ? DisplayStyle.Flex
                : DisplayStyle.None;
    }

    private void TeleportTo(Vector2 worldPosition)
    {
        playerMovement.Teleport(worldPosition);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();

        // Don't place/edit cells when clicking on the editor UI.
        if (editorOpened && IsPointerOverEditorUI(screenPosition))
        {
            return;
        }

        Vector3 worldPosition =
            playerCamera.ScreenToWorldPoint(screenPosition);

        if (editorOpened)
        {
            Vector2Int blockPosition =
                ChunkUtilities.WorldToBlockCoord(worldPosition);

            worldEditor.SetSelectedCell(blockPosition);
        }
        else
        {
            TeleportTo(
                new Vector2(
                    worldPosition.x,
                    worldPosition.y
                )
            );
        }
    }

    private bool IsPointerOverEditorUI(Vector2 screenPosition)
    {
        VisualElement root = worldEditorList.rootVisualElement;

        if (root.panel == null)
        {
            return false;
        }

        Vector2 panelPosition =
            RuntimePanelUtils.ScreenToPanel(
                root.panel,
                screenPosition
            );

        VisualElement element =
            root.panel.Pick(panelPosition);

        return element != null &&
               element != root;
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (teleportAction == null)
        {
            Debug.LogError(
                "Teleport action is not assigned.",
                this
            );

            isValid = false;
        }

        if (toggleCollision == null)
        {
            Debug.LogError(
                "ToggleCollision action is not assigned.",
                this
            );

            isValid = false;
        }

        if (toggleEditor == null)
        {
            Debug.LogError(
                "ToggleEditor action is not assigned.",
                this
            );

            isValid = false;
        }

        if (playerCamera == null)
        {
            Debug.LogError(
                "Player camera is not assigned.",
                this
            );

            isValid = false;
        }

        if (playerMovement == null)
        {
            Debug.LogError(
                "Player movement is not assigned.",
                this
            );

            isValid = false;
        }

        if (playerCollider == null)
        {
            Debug.LogError(
                "Player collider is not assigned.",
                this
            );

            isValid = false;
        }

        if (worldEditorList == null)
        {
            Debug.LogError(
                "World editor UIDocument is not assigned.",
                this
            );

            isValid = false;
        }

        if (worldEditor == null)
        {
            Debug.LogError(
                "WorldEditor is not assigned.",
                this
            );

            isValid = false;
        }

        return isValid;
    }
}