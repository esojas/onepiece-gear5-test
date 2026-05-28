using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] private float jointSpring = 4.5f;
    [SerializeField] private float jointDamper = 7f;
    [SerializeField] private float jointMassScale = 4.5f;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private float maximumDischargeAmount = 100f;
    public float maximumDischarge => maximumDischargeAmount; 

    [SerializeField] private LayerMask whatIsGrappable;
    [SerializeField] private Transform cam,player;
    [SerializeField] private Transform linePosition;
    [SerializeField] private float jointMaxDistanceSpeed = 1f;

    public event Action IsGrappling;
    //public event Action ReleasedHoldGrappling;
    public event Action CancelledGrappling;
    public event Action<float> OnChargedUpdated;

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
    private Rigidbody rb;
    // For the hand visual
    private PlayerAnimationScript playerAnimationScript;
    private AnimationScript animationScript;
    private bool initiateChargedHold = false;
    [SerializeField] private Transform handMesh;
    [SerializeField] private ChainIKConstraint rightHandIK;
    private Transform rightHandIKTarget;

    private void Awake()
    {
        playerAnimationScript = GetComponent<PlayerAnimationScript>();
        animationScript = GetComponent<AnimationScript>();
        lineRenderer = GetComponent<LineRenderer>();
        playerGameObject = gameObject;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
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
        playerInput.OnGrappledFurtherReleased += GrappledFurtherReleased;
    }

    private void OnDisable()
    {
        playerInput.OnGrappleHold -= InitializeGrappleHold;
        playerInput.OnGrappleReleased -= ReleasedGrappleHold;
        playerInput.OnGrappleCancelled -= CancelledGrapple;
        playerInput.OnGrappledCloser -= GrappledCloserPressed;
        playerInput.OnGrappledCloserReleased -= GrappledCloserReleased;
        playerInput.OnGrappledFurther -= GrappledFurtherPressed;
        playerInput.OnGrappledFurtherReleased -= GrappledFurtherReleased;
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

            OnChargedUpdated?.Invoke(amtDischarge);

            HanldeChargingAnimation();
        }
    }

    private void HanldeChargingAnimation()
    {
        if (!initiateChargedHold)
        {
            initiateChargedHold = true;
            playerAnimationScript.SetAttacking(true);
            animationScript.ChangeAnimation("luffy_initiateChargingAttack", .2f);
        }

    }

    private void ShotGrapple(float amtDischargeGiven)
    {
        playerAnimationScript.SetAttacking(true);
        animationScript.ChangeAnimation("luffy_releaseAttack", .01f);
        // add a code that sends a message to player movement script

        Debug.Log($"The force discharge is {amtDischargeGiven}");
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward,out hit, amtDischargeGiven, whatIsGrappable))
        {
            rightHandIK.weight = 1f;
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
        OnChargedUpdated?.Invoke(amtDischarge);
        rangeDischarge = 0;
        initiateChargedHold = false; 
    }

    private void DrawLine()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0,linePosition.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void CancelledGrapple()
    {
        rightHandIK.weight = 0f;
        isGrappling = false;
        CancelledGrappling?.Invoke();
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void ReleasedGrappleHold()
    {
        isHolding = false;
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

    private void GrappledFurtherPressed() => grappledFurtherPressed = true;

    private void GrappledFurtherReleased() => grappledFurtherPressed = false;

    private void HandleGrappledCloser()
    {
        if (grappledCloserPressed && !grappledFurtherPressed)
        {
            joint.maxDistance += -jointMaxDistanceSpeed * Time.deltaTime;
        }
    }

    private void HandleGrappledFurther()
    {
        if (grappledFurtherPressed && !grappledCloserPressed)
        {
            joint.maxDistance += jointMaxDistanceSpeed * Time.deltaTime;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightHandIK = GameObject.Find("Right Hand IK").GetComponent<ChainIKConstraint>();
        rightHandIKTarget = rightHandIK.transform.GetChild(0);
        handMesh = GameObject.Find("DEF-hand.R").transform;
        rightHandIK.weight = 0f;
    }

    private void LateUpdate()
    {
        if (!isGrappling) return;

        handMesh.position = grapplePoint;
        rightHandIKTarget.position = grapplePoint; // Put arm on hit point
        //handMesh.LookAt(grapplePoint);
    }

    // Update is called once per frame
    void Update()
    {
        //DrawLine();
        ChargedGrapple();
        HandleGrappledCloser();
        HandleGrappledFurther();
    }
}
