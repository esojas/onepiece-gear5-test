using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputActionAsset inputActions;

    public event Action<Vector2> OnMove;
    public event Action OnJumpPressed;

    private InputAction moveAction;
    private InputAction jumpAction;


    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();

        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        jumpAction.performed += ctx => OnJumpPressed?.Invoke();
    }

    private void OnDisable()
    {
        moveAction.performed -= ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled -= ctx => OnMove?.Invoke(Vector2.zero);
        jumpAction.performed -= ctx => OnJumpPressed?.Invoke();

        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
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
