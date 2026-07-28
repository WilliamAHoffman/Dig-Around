using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference toggleSprintAction;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();

        toggleSprintAction.action.performed += OnToggleSprint;
        toggleSprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();

        toggleSprintAction.action.performed -= OnToggleSprint;
        toggleSprintAction.action.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        playerMovement.SetMovementInput(input);
    }

    private void OnToggleSprint(InputAction.CallbackContext context)
    {
        playerMovement.ToggleSprinting();
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (moveAction == null)
        {
            Debug.LogError("Move action is not assigned.", this);
            isValid = false;
        }

        if (toggleSprintAction == null)
        {
            Debug.LogError("Toggle sprint action is not assigned.", this);
            isValid = false;
        }

        if (playerMovement == null)
        {
            Debug.LogError("Player movement is not assigned.", this);
            isValid = false;
        }

        return isValid;
    }
}