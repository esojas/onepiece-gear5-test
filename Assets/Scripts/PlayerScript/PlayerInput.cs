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
    public event Action OnGrappleHold;
    public event Action OnGrappleReleased;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction grappleAction;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();

        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        jumpAction.performed += ctx => OnJumpPressed?.Invoke();
        sprintAction.performed += ctx => OnSprintPressed?.Invoke();
        sprintAction.canceled += ctx => OnSprintReleased?.Invoke();
        grappleAction.performed += ctx => OnGrappleHold?.Invoke();
        grappleAction.canceled += ctx => OnGrappleReleased?.Invoke();
    }

    private void OnDisable()
    {
        moveAction.performed -= ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled -= ctx => OnMove?.Invoke(Vector2.zero);
        jumpAction.performed -= ctx => OnJumpPressed?.Invoke();
        sprintAction.performed -= ctx => OnSprintPressed?.Invoke();
        sprintAction.canceled -= ctx => OnSprintReleased?.Invoke();
        grappleAction.performed -= ctx => OnGrappleHold?.Invoke();
        grappleAction.canceled -= ctx => OnGrappleReleased?.Invoke();

        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        grappleAction = InputSystem.actions.FindAction("Grapple");
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
