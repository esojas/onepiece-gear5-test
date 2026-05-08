using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputActionAsset inputActions;

    public event Action<Vector2> OnMove;
    public event Action OnJumpPressed;
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

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private InputAction grappledMovementAction;
    private InputAction grappleAction;
    private InputAction cancelledGrapple;
    private InputAction grappledCloser;
    private InputAction grappledFurther;

    private Action<InputAction.CallbackContext> onMovePerformed;
    private Action<InputAction.CallbackContext> onMoveCancelled;
    private Action<InputAction.CallbackContext> onJumpPerformed;
    private Action<InputAction.CallbackContext> onSprintPerformed;
    private Action<InputAction.CallbackContext> onSprintCanceled;

    private Action<InputAction.CallbackContext> onGrappledMovementPerformed;
    private Action<InputAction.CallbackContext> onGrappledMovementCancelled;
    private Action<InputAction.CallbackContext> onGrapplePerformed;
    private Action<InputAction.CallbackContext> onGrappleReleased;
    private Action<InputAction.CallbackContext> onGrappleCancelled;
    private Action<InputAction.CallbackContext> onGrappledCloserPerformed;
    private Action<InputAction.CallbackContext> onGrappledCloserCancelled;
    private Action<InputAction.CallbackContext> onGrappledFurtherPerformed;
    private Action<InputAction.CallbackContext> onGrappledFurtherCancelled;


    private PlayerGrapple playerGrappleScript;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("GrappleControl").Disable();

        moveAction.performed += onMovePerformed;
        moveAction.canceled += onMoveCancelled;
        jumpAction.performed += onJumpPerformed;
        sprintAction.performed += onSprintPerformed;
        sprintAction.canceled += onSprintCanceled;
        
        grappleAction.performed += onGrapplePerformed;
        grappleAction.canceled += onGrappleReleased;
        cancelledGrapple.performed += onGrappleCancelled;
        grappledMovementAction.performed += onGrappledMovementPerformed;
        grappledMovementAction.canceled += onGrappledMovementCancelled;
        grappledCloser.performed += onGrappledCloserPerformed;
        grappledCloser.canceled += onGrappledCloserCancelled;
        grappledFurther.performed += onGrappledFurtherPerformed;
        grappledFurther.canceled += onGrappledFurtherCancelled;

        playerGrappleScript.CancelledGrappling += GrappleToPlayerControlSwitch;
        playerGrappleScript.IsGrappling += PlayerToGrappleControlSwitch; // I used IsGrappling because the event runs after it detects a wall and simulates the spring.
    }

    private void OnDisable()
    {
        moveAction.performed -= onMovePerformed;
        moveAction.canceled -= onMoveCancelled;
        jumpAction.performed -= onJumpPerformed;
        sprintAction.performed -= onSprintPerformed;
        sprintAction.canceled -= onSprintCanceled;

        grappleAction.performed -= onGrapplePerformed;
        grappleAction.canceled -= onGrappleReleased;
        cancelledGrapple.performed -= onGrappleCancelled;
        grappledMovementAction.performed -= onGrappledMovementPerformed;
        grappledMovementAction.canceled -= onGrappledMovementCancelled;
        grappledCloser.performed -= onGrappledCloserPerformed;
        grappledCloser.canceled -= onGrappledCloserCancelled;
        grappledFurther.performed -= onGrappledFurtherPerformed;
        grappledFurther.canceled -= onGrappledFurtherCancelled;

        playerGrappleScript.CancelledGrappling -= GrappleToPlayerControlSwitch;
        playerGrappleScript.IsGrappling -= PlayerToGrappleControlSwitch;

        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("GrappleControl").Disable();
    }

    private void Awake()
    {
        playerGrappleScript = GetComponent<PlayerGrapple>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        grappleAction = InputSystem.actions.FindAction("Grapple");
        grappledMovementAction = InputSystem.actions.FindAction("GrappledMove");
        cancelledGrapple = InputSystem.actions.FindAction("LetGoGrappled");
        grappledCloser = InputSystem.actions.FindAction("GrappledCloser");
        grappledFurther = InputSystem.actions.FindAction("GrappledFurther");

        onMovePerformed = ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>()); 
        onMoveCancelled = ctx => OnMove?.Invoke(Vector2.zero);
        onJumpPerformed = ctx => OnJumpPressed?.Invoke();
        onSprintPerformed = ctx => OnSprintPressed?.Invoke();
        onSprintCanceled = ctx => OnSprintReleased?.Invoke();
        onGrapplePerformed = ctx => OnGrappleHold?.Invoke();
        onGrappleReleased = ctx => OnGrappleReleased?.Invoke();
        onGrappledMovementPerformed = ctx => OnGrappledMovement?.Invoke(ctx.ReadValue<Vector2>());
        onGrappledMovementCancelled = ctx => OnGrappledMovement?.Invoke(Vector2.zero);
        onGrappledCloserPerformed = ctx => OnGrappledCloser?.Invoke();
        onGrappledCloserCancelled = ctx => OnGrappledCloserReleased?.Invoke();
        onGrappledFurtherPerformed = ctx => OnGrappledFurther?.Invoke();
        onGrappledFurtherCancelled = ctx => OnGrappledFurtherReleased?.Invoke();


        onGrappleCancelled = ctx => OnGrappleCancelled?.Invoke();
    }

    private void PlayerToGrappleControlSwitch()
    {
        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("GrappleControl").Enable();
    }

    private void GrappleToPlayerControlSwitch()
    {
        inputActions.FindActionMap("GrappleControl").Disable();
        inputActions.FindActionMap("Player").Enable();
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
