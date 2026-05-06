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

    public event Action IsGrappling;
    public event Action StoppedGrappling;

    private LineRenderer lineRenderer;
    private GameObject playerGameObject;
    private Vector3 grapplePoint;
    private PlayerInput playerInput;
    private SpringJoint joint;
    private bool isHolding = false;
    private float amtDischarge = 0;
    private float rangeDischarge;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerGameObject = gameObject;
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.OnGrappleHold += InitializeGrappleHold;
        playerInput.OnGrappleReleased += ReleasedGrappleHold;
    }

    private void OnDisable()
    {
        playerInput.OnGrappleHold -= InitializeGrappleHold;
        playerInput.OnGrappleReleased -= ReleasedGrappleHold;
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
        if (Physics.Raycast(cam.position, cam.forward,out hit, 100, whatIsGrappable))
        {
            IsGrappling?.Invoke();
            grapplePoint = hit.point;
            joint = playerGameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * .8f;
            joint.minDistance = distanceFromPoint * .25f;

            joint.damper = jointDamper;
            joint.spring = jointSpring;
            joint.massScale = jointMassScale;
        }

        amtDischarge = 0;
        rangeDischarge = 0;
    }

    private void DrawLine()
    {
        lineRenderer.SetPosition(0,linePosition.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ReleasedGrappleHold()
    {
        isHolding = false;

        StoppedGrappling?.Invoke();
        
        if (!isHolding) ShotGrapple(rangeDischarge);
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
    }
}
