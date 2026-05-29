using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class PlayerAttackScript : MonoBehaviour
{
    [SerializeField] MeeleeAttack punchAttackObject;
    [SerializeField] MeeleeAttack kickAttackObject;

    [SerializeField] private ChainIKConstraint rightHandIK;
    [SerializeField] private ChainIKConstraint leftHandIK;
    private Transform rightHandIKTarget;
    private Transform leftHandIKTarget;
    [SerializeField] private Transform rightPalmArmBone;
    [SerializeField] private Transform leftPalmArmBone;

    [SerializeField] private ChainIKConstraint rightLegIK;
    [SerializeField] private ChainIKConstraint leftLegIK;
    private Transform rightLegIKTarget;
    private Transform leftLegIKTarget;
    [SerializeField] private Transform rightFeetBone;
    [SerializeField] private Transform leftFeetBone;

    private PlayerInput playerInput;

    private bool punchAttackIsPressed = false;
    [SerializeField] private GameObject punchAttackGameObject;
    [SerializeField] private Transform originPunchAttack, cam;
    private float dischargePunchAmt;
    private float rangePunchDischargeAmt;

    private bool kickAttackIsPressed = false;
    [SerializeField] private GameObject kickAttackGameObject;
    [SerializeField] private Transform originKickAttack;

    private float dischargeKickAmt;
    private float rangeKickDischargeAmt;

    private Transform playerTransform;

    public event Action<float> OnFistChargedUpdated;
    public event Action<float> OnKickChargedUpdated;

    private PlayerAnimationScript playerAnimationScript;
    private AnimationScript animationScript;

    private void OnPunchAttackPressed()
    {
        if (punchCooldownTimer > 0) return; 
        punchAttackIsPressed = true;
    }

    private void OnKickAttackPressed()
    {
        if (kickCooldownTimer > 0) return; 
        kickAttackIsPressed = true;
    } 

    private bool initiatePunchAttackHold = false;
    private bool initiateKickAttackHold = false;

    private float punchCooldownTimer = 0f;
    private float kickCooldownTimer = 0f;

    private void OnEnable()
    {
        playerInput.OnPunchAttackPressed += OnPunchAttackPressed;
        playerInput.OnPunchAttackReleased += OnPunchAttackReleased;
        playerInput.OnKickAttackPressed += OnKickAttackPressed;
        playerInput.OnKickAttackReleased += OnKickAttackReleased;
    }

    private void OnDisable()
    {
        playerInput.OnPunchAttackPressed -= OnPunchAttackPressed;
        playerInput.OnPunchAttackReleased -= OnPunchAttackReleased;
        playerInput.OnKickAttackPressed -= OnKickAttackPressed;
        playerInput.OnKickAttackReleased -= OnKickAttackReleased;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerTransform = transform;
        playerAnimationScript = GetComponent<PlayerAnimationScript>();
        animationScript = GetComponent<AnimationScript>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnFistChargedUpdated?.Invoke(dischargePunchAmt);
        OnKickChargedUpdated?.Invoke(dischargeKickAmt);
        // Hands
        rightHandIK = GameObject.Find("Right Hand IK").GetComponent<ChainIKConstraint>();
        leftHandIK = GameObject.Find("Left Hand IK").GetComponent<ChainIKConstraint>();
        // Only enable them when the punch animation is executed.
        rightHandIK.weight = 0f;
        leftHandIK.weight = 0f;

        rightHandIKTarget = rightHandIK.transform.GetChild(0); 
        leftHandIKTarget = leftHandIK.transform.GetChild(0);

        //rightPalmArmBone = GameObject.Find("Luffy-DEF-hand.R").transform;
        //leftPalmArmBone = GameObject.Find("Luffy-DEF-hand.L").transform;
        // Legs
        rightLegIK = GameObject.Find("Right Leg IK").GetComponent<ChainIKConstraint>();
        leftLegIK = GameObject.Find("Left Leg IK").GetComponent<ChainIKConstraint>();
        // Only enable them when the punch animation is executed.
        rightLegIK.weight = 0f;
        leftLegIK.weight = 0f;

        rightLegIKTarget = rightLegIK.transform.GetChild(0);
        leftLegIKTarget = leftLegIK.transform.GetChild(0);

        //rightFeetBone = GameObject.Find("Luffy-DEF-toe.R").transform;
        //leftFeetBone = GameObject.Find("Luffy-DEF-toe.L").transform;

    }

    public float fistMaximumDischarge()
    {
        return punchAttackObject.attackRange;
    }

    public float kickMaximumDischarge()
    {
        return kickAttackObject.attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (punchCooldownTimer > 0) punchCooldownTimer -= Time.deltaTime; 
        if (kickCooldownTimer > 0) kickCooldownTimer -= Time.deltaTime;

        HandleChargedAttack();
        HandleInitiateAttackAnimation();
    }

    private void HandleChargedAttack()
    {
        if (punchAttackIsPressed )
        {
            dischargePunchAmt += punchAttackObject.attackChargeMultiplier * Time.deltaTime;

            OnFistChargedUpdated?.Invoke(dischargePunchAmt);

            rangePunchDischargeAmt = Mathf.Clamp(dischargePunchAmt, 12, punchAttackObject.attackRange);
        }
        if (kickAttackIsPressed)
        {
            dischargeKickAmt += kickAttackObject.attackChargeMultiplier * Time.deltaTime;

            OnKickChargedUpdated?.Invoke(dischargeKickAmt);

            rangeKickDischargeAmt = Mathf.Clamp(dischargeKickAmt, 15, kickAttackObject.attackRange);
        }
    }

    private void HandleInitiateAttackAnimation()
    {
        if(dischargePunchAmt > 5f && !initiatePunchAttackHold)
        {
            initiatePunchAttackHold = true;
            playerAnimationScript.SetAttacking(true);
            animationScript.ChangeAnimation("luffy_initiateChargingAttack", .2f);
        }
        if (dischargeKickAmt > 5f && !initiateKickAttackHold)
        {
            initiateKickAttackHold = true;
            playerAnimationScript.SetAttacking(true);
            animationScript.ChangeAnimation("luffy_initiateChargingKickAttack", .2f);
        }
    }

    private void OnKickAttackReleased()
    {
        if (!kickAttackIsPressed) return; // was blocked by cooldown, do nothing
        kickAttackIsPressed = false;
        HandleKickAttack();
    }

    private void OnPunchAttackReleased()
    {
        if (!punchAttackIsPressed) return; // was blocked by cooldown, do nothing
        punchAttackIsPressed = false;
        HandlePunchAttack();
    }

    private void HandleKickAttack()
    {
        Debug.Log($"{dischargeKickAmt} is the discharge kick amount");

        GameObject kickPrefab = Instantiate(kickAttackGameObject, originKickAttack.position, cam.rotation);
        KickProjectile kickProjectTileScript = kickPrefab.GetComponent<KickProjectile>();

        Vector3 targetPos = cam.position + (rangeKickDischargeAmt * cam.forward);

        int rand = UnityEngine.Random.Range(0, 2);

        if (dischargeKickAmt <= 5f)
        {
            playerAnimationScript.SetAttacking(true);
            if (rand == 0)
            {
                animationScript.ChangeAnimation("luffy_releaseKickAttack", .01f); // right hand
                rightLegIK.weight = 1f;
                leftLegIK.weight = 0f;
            }
            else
            {
                animationScript.ChangeAnimation("luffy_releaseKickAttack02", .01f); // left hand
                rightLegIK.weight = 0f;
                leftLegIK.weight = 1f;
            }
        }
        else
        {
            playerAnimationScript.SetAttacking(true);
            animationScript.ChangeAnimation("luffy_releaseKickAttack", .001f);
            rightLegIK.weight = 1f;
            leftLegIK.weight = 0f;
            rand = 0;
        }

        Transform feetBone = rand == 0 ? rightFeetBone : leftFeetBone;

        Transform ikTarget = rand == 0 ? rightLegIKTarget : leftLegIKTarget;

        kickProjectTileScript.InitializeKickProjectile(kickAttackObject, targetPos, playerTransform, ikTarget, feetBone);

        kickProjectTileScript.OnKickDestroyed += () =>
        {
            rightLegIK.weight = 0f;
            leftLegIK.weight = 0f;
        };

        dischargeKickAmt = 0;
        OnKickChargedUpdated?.Invoke(dischargeKickAmt);
        rangeKickDischargeAmt = 0;
        initiateKickAttackHold = false;
        kickCooldownTimer = kickAttackObject.attackCooldown;
    }

    private void HandlePunchAttack()
    {
        Debug.Log($"{dischargePunchAmt} is the discharge punch amount");

        GameObject fistPrefab = Instantiate(punchAttackGameObject, originPunchAttack.position, cam.rotation);
        FistProjectile fistProjectTileScript = fistPrefab.GetComponent<FistProjectile>();

        Vector3 targetPos = cam.position + (rangePunchDischargeAmt * cam.forward);

        int rand = UnityEngine.Random.Range(0, 2);

        if (dischargePunchAmt <= 5f)
        {
            playerAnimationScript.SetAttacking(true);
            if (rand == 0)
            {
                animationScript.ChangeAnimation("luffy_attack_01", .01f); // right hand
                rightHandIK.weight = 1f;
                leftHandIK.weight = 0f;
            }
            else
            {
                animationScript.ChangeAnimation("luffy_attack_02", .01f); // left hand
                rightHandIK.weight = 0f;
                leftHandIK.weight = 1f;
            }
        }
        else
        {
            playerAnimationScript.SetAttacking(true);
            animationScript.ChangeAnimation("luffy_releaseAttack", .01f);
            rightHandIK.weight = 1f;
            leftHandIK.weight = 0f;
            rand = 0;
        }

        Transform palmBone = rand == 0 ? rightPalmArmBone : leftPalmArmBone; // the bone that stretch

        Transform ikTarget = rand == 0 ? rightHandIKTarget : leftHandIKTarget;

        fistProjectTileScript.InitializeFistProjectile(punchAttackObject, targetPos, playerTransform, ikTarget, palmBone);

        fistProjectTileScript.OnFistDestroyed += () =>
        {
            rightHandIK.weight = 0f;
            leftHandIK.weight = 0f;
        };

        dischargePunchAmt = 0;
        OnFistChargedUpdated?.Invoke(dischargePunchAmt);
        rangePunchDischargeAmt = 0;
        initiatePunchAttackHold = false;
        punchCooldownTimer = punchAttackObject.attackCooldown;
    }




}
