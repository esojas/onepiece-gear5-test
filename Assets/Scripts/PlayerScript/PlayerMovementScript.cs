using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 2f;
    [SerializeField] private float jumpForce = 1f;
    private float interpolateMovementSpeed; // The time it takes for the player to move smoothly from their current speed to the movement speed
    [SerializeField] private float slowInterpolateSpeed = 0.05f;
    [SerializeField] private float fastInterpolateSpeed = 0.5f;
    [SerializeField] private float lookRotationSpeed = 3f;
    [SerializeField] private float flightDuration = 7f;
    [SerializeField] private float flightCooldown = 7f;


    private bool toggleFly = false;
    private bool canFly = false;
    private bool spacedHold = false;

    private Coroutine flyTimerCoroutine;
    private Coroutine momentumCoroutine;

    private bool isPlayerSprinting = false;
    private PlayerInput playerInput;
    private Vector2 direction;

    public bool playerIsGrappling = false;
    [SerializeField] private Camera cam;

    Transform cameraPosition;
    Transform relativeCameraPosition;
    PlayerGrapple playerGrappleScript;

    private Rigidbody rb;

    private PlayerDrawingScript playerDrawingScript;
    private bool isDrawing = false;

    private void Start()
    {
        //for now
        Cursor.lockState = CursorLockMode.Locked;
        cameraPosition = GetComponent<Transform>();
        ThirdPersonCamera.Instance.SetCameraTarget(cameraPosition);
        relativeCameraPosition = ThirdPersonCamera.Instance.ReturnRelativeCamPos();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canFly = true;
        interpolateMovementSpeed = fastInterpolateSpeed;
        playerGrappleScript = GetComponent<PlayerGrapple>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerDrawingScript = GetComponent<PlayerDrawingScript>();
    }

    private void OnEnable()
    {
        playerInput.OnMove += HandleMovement;
        playerInput.OnJumpPressed += OnJumpPressed;
        playerInput.OnJumpReleased += OnJumpReleased;
        playerInput.OnSprintPressed += OnSprintPressed;
        playerInput.OnSprintReleased += OnSprintReleased;
        playerInput.OnGrappledMovement += HandleMovement;

        playerGrappleScript.IsGrappling += IsCurrentlyGrappling;
        playerGrappleScript.CancelledGrappling += StoppedGrappling;

        playerDrawingScript.IsDrawing += IsCurrentlyDrawing;
        playerDrawingScript.CancelledDrawing += StoppedDrawing;

        playerInput.OnToggleFly += ToggleFly;
    }

    private void OnDisable()
    {
        playerInput.OnMove -= HandleMovement;
        playerInput.OnJumpPressed -= OnJumpPressed;
        playerInput.OnSprintPressed -= OnSprintPressed;
        playerInput.OnSprintReleased -= OnSprintReleased;
        playerInput.OnGrappledMovement -= HandleMovement;

        playerGrappleScript.IsGrappling -= IsCurrentlyGrappling;
        playerGrappleScript.CancelledGrappling -= StoppedGrappling;

        playerDrawingScript.IsDrawing -= IsCurrentlyDrawing;
        playerDrawingScript.CancelledDrawing -= StoppedDrawing;
    }

    private void ToggleFly()
    {
        if (canFly)
        {
            if (flyTimerCoroutine != null) StopCoroutine(flyTimerCoroutine);
            if (momentumCoroutine != null) StopCoroutine(momentumCoroutine);

            toggleFly = !toggleFly;
            rb.useGravity = !rb.useGravity;

            if (toggleFly) 
            {
                flyTimerCoroutine = StartCoroutine(StartToggleFlyTimer(flightDuration));
            }
            else 
            {
                canFly = false;
                StartCoroutine(FlightCooldown());
            }
        }
    }

    private void IsCurrentlyGrappling()
    {
        playerIsGrappling = true;
        rb.useGravity = true;
        toggleFly = false;
    }
    private void IsCurrentlyDrawing() => isDrawing = true;

    private void StoppedGrappling()
    {
        playerIsGrappling = false;
        rb.useGravity = true;
        toggleFly = false;
        StartCoroutine(StartCoroutineMomentumTimer());
    }

    private void StoppedDrawing() => isDrawing = false;
    
    private void OnSprintPressed() => isPlayerSprinting = true;

    private void OnSprintReleased() => isPlayerSprinting = false;

    private void OnJumpPressed()
    {
        spacedHold = true;
        if (!toggleFly)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void OnJumpReleased()
    {
        spacedHold = false;
    }

    private void HandleFlightMovement()
    {
        if (toggleFly && spacedHold)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void HandleMovement(Vector2 dir)
    {
        direction = dir;
    }

    private void HandleMoveDirAndMoveSpeed()
    {
        Vector3 moveDirection;

        if (toggleFly)
        {
            moveDirection = (direction.x * relativeCameraPosition.right) + (direction.y * relativeCameraPosition.forward);
        }
        else
        {
            moveDirection = (direction.x * relativeCameraPosition.right) + (direction.y * relativeCameraPosition.forward);
            moveDirection.y = 0;
        }

        float finalSpeed = isPlayerSprinting ? sprintSpeed : movementSpeed;

        Movement(moveDirection,finalSpeed);
        playerGrappleScript.GrappleMovement(rb, moveDirection,finalSpeed);
    }

    private void HandleRotation()
    {
        Quaternion target;
        Vector3 flatForward = relativeCameraPosition.forward;
        flatForward.y = 0; 
        flatForward.Normalize();
        if (flatForward != Vector3.zero)
        {
            target = Quaternion.LookRotation(flatForward);
        }
        else
        {
            target = transform.rotation;
        }

        float speed = lookRotationSpeed * Time.deltaTime;

        Quaternion current = transform.rotation;

        transform.rotation = Quaternion.Slerp(current, target, speed);
    }

    private void Movement(Vector3 moveDirection, float finalSpeed)
    {
        if (!playerIsGrappling)
        {
            float targetX = finalSpeed * moveDirection.x;
            float targetZ = finalSpeed * moveDirection.z;
            float targetY = toggleFly ? finalSpeed * moveDirection.y : rb.linearVelocity.y; // lerp Y only during fly

            rb.linearVelocity = new Vector3(
                Mathf.Lerp(rb.linearVelocity.x, targetX, interpolateMovementSpeed),
                toggleFly ? Mathf.Lerp(rb.linearVelocity.y, targetY, interpolateMovementSpeed) : rb.linearVelocity.y,
                Mathf.Lerp(rb.linearVelocity.z, targetZ, interpolateMovementSpeed)
            );
        }
    }

    private IEnumerator StartToggleFlyTimer(float duration)
    {
        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        rb.useGravity = true;
        toggleFly = false;
        canFly = false;
        StartCoroutine(FlightCooldown());
    }

    private IEnumerator FlightCooldown()
    {
        yield return new WaitForSeconds(flightCooldown);
        canFly = true; 
    }

    private IEnumerator StartCoroutineMomentumTimer()
    {
        interpolateMovementSpeed = slowInterpolateSpeed;

        while (interpolateMovementSpeed < fastInterpolateSpeed)
        {
            interpolateMovementSpeed += Time.deltaTime * 0.05f;
            interpolateMovementSpeed = Mathf.Clamp(interpolateMovementSpeed, slowInterpolateSpeed, fastInterpolateSpeed);
            yield return null;
        }

        interpolateMovementSpeed = fastInterpolateSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleFlightMovement();
        HandleMoveDirAndMoveSpeed();
    }

    private void Update()
    {
        if (!isDrawing) // Make it so that the model doesnt also rotate during the drawing mode.
        {
            HandleRotation();
        }

    }
}
