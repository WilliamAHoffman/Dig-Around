using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class PlayerDebugCommands : MonoBehaviour
{
    #region Input

    [Header("Input")]
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private InputActionReference toggleCollision;
    [SerializeField] private InputActionReference toggleEditor;

    #endregion

    #region References

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private UIDocument worldEditorList;
    [SerializeField] private WorldEditor worldEditor;

    #endregion

    #region State

    private bool editorOpened;
    private bool clickStartedOnUI;

    private Vector2Int? lastPlacedBlock;

    private VisualElement editorPanel;

    #endregion

    #region Unity Lifecycle

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

        editorPanel = worldEditorList.rootVisualElement
            .Q<VisualElement>("WorldEditorPanel");

        if (editorPanel == null)
        {
            Debug.LogError(
                "Could not find a VisualElement named 'WorldEditorPanel'.",
                this
            );
        }

        editorOpened = false;
        clickStartedOnUI = false;
        lastPlacedBlock = null;

        worldEditorList.rootVisualElement.style.display =
            DisplayStyle.None;
    }

    private void OnEnable()
    {
        clickAction.action.performed += OnClick;
        clickAction.action.Enable();

        toggleCollision.action.performed += ToggleIntangible;
        toggleCollision.action.Enable();

        toggleEditor.action.performed += ToggleEditor;
        toggleEditor.action.Enable();
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.action.performed -= OnClick;
            clickAction.action.Disable();
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

    private void Update()
    {
        if (clickAction == null || Mouse.current == null)
        {
            return;
        }

        if (clickAction.action.WasPressedThisFrame())
        {
            Vector2 screenPosition =
                Mouse.current.position.ReadValue();

            clickStartedOnUI =
                editorOpened &&
                IsPointerOverEditorUI(screenPosition);

            lastPlacedBlock = null;
        }

        if (clickAction.action.IsPressed())
        {
            if (editorOpened && !clickStartedOnUI)
            {
                PlaceBlocks();
            }
        }

        if (clickAction.action.WasReleasedThisFrame())
        {
            clickStartedOnUI = false;
            lastPlacedBlock = null;
        }
    }

    #endregion

    #region Collision

    public void SetIntangible(bool isIntangible)
    {
        playerCollider.enabled = !isIntangible;

        UpdatePlayerTransparency();
    }

    private void ToggleIntangible(
        InputAction.CallbackContext context
    )
    {
        playerCollider.enabled = !playerCollider.enabled;

        UpdatePlayerTransparency();

        Debug.Log(
            $"Player collider switched to: {playerCollider.enabled}",
            this
        );
    }

    private void UpdatePlayerTransparency()
    {
        if (playerRenderer == null)
        {
            return;
        }

        Color color = playerRenderer.color;

        color.a = playerCollider.enabled
            ? 1f
            : 0.5f;

        playerRenderer.color = color;
    }

    #endregion

    #region Editor

    private void ToggleEditor(
        InputAction.CallbackContext context
    )
    {
        editorOpened = !editorOpened;

        worldEditorList.rootVisualElement.style.display =
            editorOpened
                ? DisplayStyle.Flex
                : DisplayStyle.None;

        clickStartedOnUI = false;
        lastPlacedBlock = null;
    }

    private void PlaceBlocks()
    {
        if (!editorOpened || Mouse.current == null)
        {
            return;
        }

        Vector2 screenPosition =
            Mouse.current.position.ReadValue();

        if (IsPointerOverEditorUI(screenPosition))
        {
            return;
        }

        Vector3 worldPosition =
            playerCamera.ScreenToWorldPoint(screenPosition);

        Vector2Int blockPosition =
            ChunkUtilities.WorldToBlockCoord(worldPosition);

        if (lastPlacedBlock.HasValue &&
            lastPlacedBlock.Value == blockPosition)
        {
            return;
        }

        worldEditor.SetSelectedCell(blockPosition);

        lastPlacedBlock = blockPosition;
    }

    private bool IsPointerOverEditorUI(
        Vector2 screenPosition
    )
    {
        if (!editorOpened ||
            editorPanel == null ||
            editorPanel.panel == null)
        {
            return false;
        }

        Vector2 panelPosition =
            RuntimePanelUtils.ScreenToPanel(
                editorPanel.panel,
                screenPosition
            );

        return editorPanel.worldBound.Contains(panelPosition);
    }

    #endregion

    #region On Click

    private void TeleportTo(Vector2 worldPosition)
    {
        playerMovement.Teleport(worldPosition);
    }

    private void OnClick(
        InputAction.CallbackContext context
    )
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 screenPosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            playerCamera.ScreenToWorldPoint(screenPosition);


        TeleportTo(
            new Vector2(
                worldPosition.x,
                worldPosition.y
            )
        );
    }

    #endregion

    #region Validation

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (clickAction == null)
        {
            Debug.LogError(
                "ClickAction is not assigned.",
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

        if (playerRenderer == null)
        {
            Debug.LogError(
                "Player renderer is not assigned.",
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

    #endregion
}
