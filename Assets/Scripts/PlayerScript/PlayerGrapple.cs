using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] private float jointSpring = 4.5f;
    [SerializeField] private float jointDamper = 7f;
    [SerializeField] private float jointMassScale = 4.5f;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private float maximumDischargeAmount = 10f;
    [SerializeField] private LayerMask whatIsGrappable;
    [SerializeField] private Transform cam,player;
    [SerializeField] private Transform linePosition;
    [SerializeField] private float jointMaxDistanceSpeed = 1f;

    public event Action IsGrappling;
    //public event Action ReleasedHoldGrappling;
    public event Action CancelledGrappling;

    private bool isGrappling = false;
    private LineRenderer lineRenderer;
    private GameObject playerGameObject;
    private Vector3 grapplePoint;
    private PlayerInput playerInput;
    //private PlayerMovementScript playerMovementScript;
    private SpringJoint joint;
    private bool isHolding = false;
    private float amtDischarge = 0;
    private float rangeDischarge;
    private bool grappledCloserPressed = false;
    private bool grappledFurtherPressed = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerGameObject = gameObject;
        playerInput = GetComponent<PlayerInput>();
        //playerMovementScript = GetComponent<PlayerMovementScript>();
    }

    private void OnEnable()
    {
        playerInput.OnGrappleHold += InitializeGrappleHold;
        playerInput.OnGrappleReleased += ReleasedGrappleHold;
        playerInput.OnGrappleCancelled += CancelledGrapple;
        playerInput.OnGrappledCloser += GrappledCloserPressed;
        playerInput.OnGrappledCloserReleased += GrappledCloserReleased;
        playerInput.OnGrappledFurther += GrappledFurtherPressed;
        playerInput.OnGrappledFurther += GrappledFurtherReleased;
    }

    private void OnDisable()
    {
        playerInput.OnGrappleHold -= InitializeGrappleHold;
        playerInput.OnGrappleReleased -= ReleasedGrappleHold;
        playerInput.OnGrappleCancelled -= CancelledGrapple;
        playerInput.OnGrappledCloser -= GrappledCloserPressed;
        playerInput.OnGrappledCloserReleased -= GrappledCloserReleased;
        playerInput.OnGrappledFurther -= GrappledFurtherPressed;
        playerInput.OnGrappledFurther -= GrappledFurtherReleased;
    }

    private void InitializeGrappleHold()
    {
        isHolding = true;
    }

    private void ChargedGrapple()
    {
        if (isHolding)
        {
            amtDischarge += chargeSpeed * Time.deltaTime;

            rangeDischarge = Mathf.Clamp(amtDischarge, 0, maximumDischargeAmount);
        }
    }

    private void ShotGrapple(float amtDischargeGiven)
    {
        // add a code that sends a message to player movement script

        Debug.Log($"The force discharge is {amtDischargeGiven}");
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward,out hit, amtDischargeGiven, whatIsGrappable))
        {
            isGrappling = true;
            IsGrappling?.Invoke();
            
            grapplePoint = hit.point;
            joint = playerGameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * .8f;
            joint.minDistance = 2f;

            lineRenderer.positionCount = 2;
            joint.damper = jointDamper;
            joint.spring = jointSpring;
            joint.massScale = jointMassScale;
        }

        amtDischarge = 0;
        rangeDischarge = 0;
    }

    private void DrawLine()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0,linePosition.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void CancelledGrapple()
    {
        isGrappling = false;
        CancelledGrappling?.Invoke();
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void ReleasedGrappleHold()
    {
        isHolding = false;
        //ReleasedHoldGrappling?.Invoke();
        if (!isHolding) ShotGrapple(rangeDischarge);
    }

    public void GrappleMovement(Rigidbody rb, Vector3 moveDirection, float finalSpeed)
    {
        if (isGrappling)
        {
            rb.AddForce(new Vector3(finalSpeed * moveDirection.x, 0, finalSpeed * moveDirection.z));
        }
    }

    private void GrappledCloserPressed() => grappledCloserPressed = true;

    private void GrappledCloserReleased() => grappledCloserPressed = false;

    private void GrappledFurtherPressed() => grappledFurtherPressed = false;

    private void GrappledFurtherReleased() => grappledFurtherPressed = true;

    private void HandleGrappledCloser()
    {
        if (grappledCloserPressed)
        {
            joint.maxDistance += -jointMaxDistanceSpeed * Time.deltaTime;
        }
    }

    private void HandleGrappledFurther()
    {
        if (grappledFurtherPressed)
        {
            joint.maxDistance += jointMaxDistanceSpeed * Time.deltaTime;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
        ChargedGrapple();
        HandleGrappledCloser();
        HandleGrappledFurther();
    }
}
