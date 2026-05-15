using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputActionAsset inputActions;

    public event Action<Vector2> OnMove;
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;

    public event Action<Vector2> OnGrappledMovement;
    public event Action OnGrappleHold;
    public event Action OnGrappleReleased;
    public event Action OnGrappleCancelled;
    public event Action OnGrappledCloser;
    public event Action OnGrappledCloserReleased;
    public event Action OnGrappledFurther;
    public event Action OnGrappledFurtherReleased;

    public event Action OnPunchAttackPressed;
    public event Action OnPunchAttackReleased;
    public event Action OnKickAttackPressed;
    public event Action OnKickAttackReleased;

    public event Action OnSwitchPlayerToDrawingMode;
    public event Action OnDrawingPressed;
    public event Action OnDrawingReleased;
    public event Action OnSwitchDrawingToPlayerMode;

    public event Action OnToggleFly;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private InputAction grappledMovementAction;
    private InputAction grappleAction;
    private InputAction cancelledGrapple;
    private InputAction grappledCloser;
    private InputAction grappledFurther;

    private InputAction punchAttackAction;
    private InputAction kickAttackAction;

    private InputAction switchPlayerToDrawingAction;
    private InputAction drawingAction;
    private InputAction switchDrawingToPlayerAction;

    private InputAction toggleFlyAction;

    private Action<InputAction.CallbackContext> onMovePerformed;
    private Action<InputAction.CallbackContext> onMoveCancelled;
    private Action<InputAction.CallbackContext> onJumpPerformed;
    private Action<InputAction.CallbackContext> onJumpCancelled;
    private Action<InputAction.CallbackContext> onSprintPerformed;
    private Action<InputAction.CallbackContext> onSprintCancelled;

    private Action<InputAction.CallbackContext> onGrappledMovementPerformed;
    private Action<InputAction.CallbackContext> onGrappledMovementCancelled;
    private Action<InputAction.CallbackContext> onGrapplePerformed;
    private Action<InputAction.CallbackContext> onGrappleReleased;
    private Action<InputAction.CallbackContext> onGrappleCancelled;
    private Action<InputAction.CallbackContext> onGrappledCloserPerformed;
    private Action<InputAction.CallbackContext> onGrappledCloserCancelled;
    private Action<InputAction.CallbackContext> onGrappledFurtherPerformed;
    private Action<InputAction.CallbackContext> onGrappledFurtherCancelled;

    private Action<InputAction.CallbackContext> onPunchAttackPerformed;
    private Action<InputAction.CallbackContext> onPunchAttackCancelled;
    private Action<InputAction.CallbackContext> onKickAttackPerformed;
    private Action<InputAction.CallbackContext> onKickAttackCancelled;

    private Action<InputAction.CallbackContext> onSwitchPlayerToDrawingPerformed;
    private Action<InputAction.CallbackContext> onDrawingPerformed;
    private Action<InputAction.CallbackContext> onDrawingCancelled;
    private Action<InputAction.CallbackContext> onSwitchDrawingToPlayerPerformed;

    private Action<InputAction.CallbackContext> onToggleFlyPerformed;

    private PlayerGrapple playerGrappleScript;

    private PlayerDrawingScript playerDrawingScript;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("DrawingControl").Disable();
        // Base Movement Controls
        moveAction.performed += onMovePerformed;
        moveAction.canceled += onMoveCancelled;
        jumpAction.performed += onJumpPerformed;
        jumpAction.canceled += onJumpCancelled;
        sprintAction.performed += onSprintPerformed;
        sprintAction.canceled += onSprintCancelled;
        // Grapple Controls
        grappleAction.performed += onGrapplePerformed;
        grappleAction.canceled += onGrappleReleased;
        cancelledGrapple.performed += onGrappleCancelled;
        grappledMovementAction.performed += onGrappledMovementPerformed;
        grappledMovementAction.canceled += onGrappledMovementCancelled;
        grappledCloser.performed += onGrappledCloserPerformed;
        grappledCloser.canceled += onGrappledCloserCancelled;
        grappledFurther.performed += onGrappledFurtherPerformed;
        grappledFurther.canceled += onGrappledFurtherCancelled;
        // Meelee Controls
        punchAttackAction.performed += onPunchAttackPerformed;
        punchAttackAction.canceled += onPunchAttackCancelled;
        kickAttackAction.performed += onKickAttackPerformed;
        kickAttackAction.canceled += onKickAttackCancelled;
        // Drawing Controls (Cancelled)
        switchPlayerToDrawingAction.performed += onSwitchPlayerToDrawingPerformed;
        switchDrawingToPlayerAction.performed += onSwitchDrawingToPlayerPerformed;
        drawingAction.performed += onDrawingPerformed;
        drawingAction.canceled += onDrawingCancelled;

        toggleFlyAction.performed += onToggleFlyPerformed;

        playerGrappleScript.CancelledGrappling += GrappleToPlayerControlSwitch;
        playerGrappleScript.IsGrappling += PlayerToGrappleControlSwitch; // I used IsGrappling because the event runs after it detects a wall and simulates the spring.

        playerDrawingScript.IsDrawing += PlayerToDrawingControlSwitch;
        playerDrawingScript.CancelledDrawing += DrawingToPlayerControlSwitch;
    }

    private void OnDisable()
    {
        moveAction.performed -= onMovePerformed;
        moveAction.canceled -= onMoveCancelled;
        jumpAction.performed -= onJumpPerformed;
        jumpAction.canceled -= onJumpCancelled;
        sprintAction.performed -= onSprintPerformed;
        sprintAction.canceled -= onSprintCancelled;

        grappleAction.performed -= onGrapplePerformed;
        grappleAction.canceled -= onGrappleReleased;
        cancelledGrapple.performed -= onGrappleCancelled;
        grappledMovementAction.performed -= onGrappledMovementPerformed;
        grappledMovementAction.canceled -= onGrappledMovementCancelled;
        grappledCloser.performed -= onGrappledCloserPerformed;
        grappledCloser.canceled -= onGrappledCloserCancelled;
        grappledFurther.performed -= onGrappledFurtherPerformed;
        grappledFurther.canceled -= onGrappledFurtherCancelled;

        punchAttackAction.performed -= onPunchAttackPerformed;
        punchAttackAction.canceled -= onPunchAttackCancelled;
        kickAttackAction.performed -= onKickAttackPerformed;
        kickAttackAction.canceled -= onKickAttackCancelled;

        switchPlayerToDrawingAction.performed -= onSwitchPlayerToDrawingPerformed;
        switchDrawingToPlayerAction.performed -= onSwitchDrawingToPlayerPerformed;
        drawingAction.performed -= onDrawingPerformed;
        drawingAction.canceled -= onDrawingCancelled;

        toggleFlyAction.performed -= onToggleFlyPerformed;

        playerGrappleScript.CancelledGrappling -= GrappleToPlayerControlSwitch;
        playerGrappleScript.IsGrappling -= PlayerToGrappleControlSwitch;

        playerDrawingScript.IsDrawing -= PlayerToDrawingControlSwitch;
        playerDrawingScript.CancelledDrawing -= DrawingToPlayerControlSwitch;

        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("DrawingControl").Disable();
    }

    private void Awake()
    {
        playerGrappleScript = GetComponent<PlayerGrapple>();
        playerDrawingScript = GetComponent<PlayerDrawingScript>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        
        grappleAction = InputSystem.actions.FindAction("Grapple");
        grappledMovementAction = InputSystem.actions.FindAction("GrappledMove");
        cancelledGrapple = InputSystem.actions.FindAction("LetGoGrappled");
        grappledCloser = InputSystem.actions.FindAction("GrappledCloser");
        grappledFurther = InputSystem.actions.FindAction("GrappledFurther");

        punchAttackAction = InputSystem.actions.FindAction("PunchAttack");
        kickAttackAction = InputSystem.actions.FindAction("KickAttack");

        switchPlayerToDrawingAction = InputSystem.actions.FindAction("DrawAbility");
        drawingAction = InputSystem.actions.FindAction("Drawing");
        switchDrawingToPlayerAction = InputSystem.actions.FindAction("FinishDrawing");

        toggleFlyAction = InputSystem.actions.FindAction("ToggleFly");

        onMovePerformed = ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>()); 
        onMoveCancelled = ctx => OnMove?.Invoke(Vector2.zero);
        onJumpPerformed = ctx => OnJumpPressed?.Invoke();
        onJumpCancelled = ctx => OnJumpReleased?.Invoke();
        onSprintPerformed = ctx => OnSprintPressed?.Invoke();
        onSprintCancelled = ctx => OnSprintReleased?.Invoke();

        onGrapplePerformed = ctx => OnGrappleHold?.Invoke();
        onGrappleReleased = ctx => OnGrappleReleased?.Invoke();
        onGrappledMovementPerformed = ctx => OnGrappledMovement?.Invoke(ctx.ReadValue<Vector2>());
        onGrappledMovementCancelled = ctx => OnGrappledMovement?.Invoke(Vector2.zero);
        onGrappledCloserPerformed = ctx => OnGrappledCloser?.Invoke();
        onGrappledCloserCancelled = ctx => OnGrappledCloserReleased?.Invoke();
        onGrappledFurtherPerformed = ctx => OnGrappledFurther?.Invoke();
        onGrappledFurtherCancelled = ctx => OnGrappledFurtherReleased?.Invoke();
        onGrappleCancelled = ctx => OnGrappleCancelled?.Invoke();

        onPunchAttackPerformed = ctx => OnPunchAttackPressed?.Invoke();
        onPunchAttackCancelled = ctx => OnPunchAttackReleased?.Invoke();
        onKickAttackPerformed = ctx => OnKickAttackPressed?.Invoke();
        onKickAttackCancelled = ctx => OnKickAttackReleased?.Invoke();

        onSwitchPlayerToDrawingPerformed = ctx => OnSwitchPlayerToDrawingMode?.Invoke();
        onDrawingPerformed = ctx => OnDrawingPressed?.Invoke();
        onDrawingCancelled = ctx => OnDrawingReleased?.Invoke();
        onSwitchDrawingToPlayerPerformed = ctx => OnSwitchDrawingToPlayerMode?.Invoke();

        onToggleFlyPerformed = ctx => OnToggleFly?.Invoke();
    }

    private void PlayerToGrappleControlSwitch()
    {
        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("GrappleControl").Enable();
        inputActions.FindActionMap("DrawingControl").Disable();
    }

    private void GrappleToPlayerControlSwitch()
    {
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("DrawingControl").Disable();
    }

    private void PlayerToDrawingControlSwitch()
    {
        Cursor.lockState = CursorLockMode.None;
        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("DrawingControl").Enable();
    }

    private void DrawingToPlayerControlSwitch()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("DrawingControl").Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
