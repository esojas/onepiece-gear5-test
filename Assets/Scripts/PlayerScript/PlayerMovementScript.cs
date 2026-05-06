using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 1f;
 
    private PlayerInput playerInput;
    private Vector2 direction;

    Transform cameraPosition;
    Transform relativeCameraPosition;

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
        rb= GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.OnMove += HandleMovement;
        playerInput.OnJumpPressed += HandleJump;
    }

    private void OnDisable()
    {
        playerInput.OnMove -= HandleMovement;
        playerInput.OnJumpPressed -= HandleJump;
    }

    private void HandleJump()
    {
        rb.AddForce(new Vector3(0,jumpForce,0), ForceMode.Impulse);
    }

    private void HandleMovement(Vector2 dir)
    {
        direction = dir;
    }

    private void Movement()
    {

        Vector3 moveDirection = (direction.x * relativeCameraPosition.right)+(direction.y * relativeCameraPosition.forward);

        moveDirection.y = 0;

        rb.linearVelocity = new Vector3(speed * moveDirection.x, rb.linearVelocity.y,speed * moveDirection.z);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
}
