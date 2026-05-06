using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 2f;
    [SerializeField] private float jumpForce = 1f;

    private bool isPlayerSprinting = false;
    private PlayerInput playerInput;
    private Vector2 direction;
    private bool playerIsMoving = false;

    private bool playerIsGrappling = false;
    Transform cameraPosition;
    Transform relativeCameraPosition;
    PlayerGrapple playerGrappleScript;

    private Rigidbody rb;

    private void Start()
    {
        cameraPosition = GetComponent<Transform>();
        ThirdPersonCamera.Instance.SetCameraTarget(cameraPosition);
        relativeCameraPosition = ThirdPersonCamera.Instance.ReturnRelativeCamPos();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerGrappleScript = GetComponent<PlayerGrapple>();
        rb= GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.OnMove += HandleMovement;
        playerInput.OnJumpPressed += HandleJump;
        playerInput.OnSprintPressed += OnSprintPressed;
        playerInput.OnSprintReleased += OnSprintReleased;
        playerGrappleScript.IsGrappling += IsCurrentlyGrappling;
        playerGrappleScript.StoppedGrappling += StoppedGrappling;
    }

    private void OnDisable()
    {
        playerInput.OnMove -= HandleMovement;
        playerInput.OnJumpPressed -= HandleJump;
        playerInput.OnSprintPressed -= OnSprintPressed;
        playerInput.OnSprintReleased -= OnSprintReleased;
        playerGrappleScript.IsGrappling -= IsCurrentlyGrappling;
        playerGrappleScript.StoppedGrappling -= StoppedGrappling;
    }

    private void IsCurrentlyGrappling() => playerIsGrappling = true;

    private void StoppedGrappling() => playerIsGrappling = false;

    private void OnSprintPressed() => isPlayerSprinting = true;

    private void OnSprintReleased() => isPlayerSprinting = false;

    private void HandleJump()
    {
        rb.AddForce(new Vector3(0,jumpForce,0), ForceMode.Impulse);
    }

    private void HandleMovement(Vector2 dir)
    {
        direction = dir;
        playerIsMoving = true;
    }

    private void Movement()
    {

        Vector3 moveDirection = (direction.x * relativeCameraPosition.right)+(direction.y * relativeCameraPosition.forward);

        moveDirection.y = 0;

        float finalSpeed = isPlayerSprinting ? sprintSpeed : movementSpeed;

        if (!playerIsGrappling)
        {
            rb.linearVelocity = new Vector3(finalSpeed * moveDirection.x, rb.linearVelocity.y, finalSpeed * moveDirection.z);
        }
        else
        {
            rb.AddForce(new Vector3(finalSpeed * moveDirection.x, 0, finalSpeed * moveDirection.z));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
}
