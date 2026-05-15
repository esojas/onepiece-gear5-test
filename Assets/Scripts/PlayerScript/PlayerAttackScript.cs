using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAttackScript : MonoBehaviour
{
    [SerializeField] MeeleeAttack punchAttackObject;
    [SerializeField] MeeleeAttack kickAttackObject;

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

    private void OnPunchAttackPressed() => punchAttackIsPressed = true;

    private void OnKickAttackPressed() => kickAttackIsPressed = true;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleChargedAttack();
    }

    private void HandleChargedAttack()
    {
        if (punchAttackIsPressed)
        {
            dischargePunchAmt += punchAttackObject.attackChargeMultiplier * Time.deltaTime;

            rangePunchDischargeAmt = Mathf.Clamp(dischargePunchAmt, 12, punchAttackObject.attackRange);
        }
        if (kickAttackIsPressed)
        {
            dischargeKickAmt += kickAttackObject.attackChargeMultiplier * Time.deltaTime;

            rangeKickDischargeAmt = Mathf.Clamp(dischargeKickAmt, 15, kickAttackObject.attackRange);
        }
    }

    private void OnKickAttackReleased()
    {
        kickAttackIsPressed = false;
        if (!kickAttackIsPressed)
        {
            HandleKickAttack();
        }
    }

    private void OnPunchAttackReleased()
    {
        punchAttackIsPressed = false;
        if (!punchAttackIsPressed)
        {
            HandlePunchAttack();
        }
    }

    private void HandleKickAttack()
    {
        GameObject kickPrefab = Instantiate(kickAttackGameObject, originKickAttack.position, cam.rotation);
        KickProjectile kickProjectTileScript = kickPrefab.GetComponent<KickProjectile>();

        Vector3 targetPos = cam.position + (rangeKickDischargeAmt * cam.forward);

        kickProjectTileScript.InitializeKickProjectile(kickAttackObject, targetPos, playerTransform);

        dischargeKickAmt = 0;
        rangeKickDischargeAmt = 0;
    }

    private void HandlePunchAttack()
    {
        Debug.Log("PUNCH! LAUNCHED!");

        GameObject fistPrefab = Instantiate(punchAttackGameObject, originPunchAttack.position, cam.rotation);
        FistProjectile fistProjectTileScript = fistPrefab.GetComponent<FistProjectile>();

        Vector3 targetPos = cam.position + (rangePunchDischargeAmt * cam.forward);

        fistProjectTileScript.InitializeFistProjectile(punchAttackObject, targetPos, playerTransform);

        dischargePunchAmt = 0;
        rangePunchDischargeAmt = 0;
    }

}
