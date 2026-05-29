using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerGiantScript : MonoBehaviour
{

    [SerializeField] private float sizeMultiplier;
    [SerializeField] private float transformSpeed;
    [SerializeField] private float giantDuration;
    [SerializeField] private float giantCooldown;
    [SerializeField] private float knockback; // the knockback when going back to normal size
    [SerializeField] GameObject smokePos;
    [SerializeField] GameObject smokeParticle;
    [SerializeField] private Transform camPostion;

    private GameObject activeSmokeInstance;
    private Vector3 normalSize;
    private PlayerInput playerInputScript;
    private bool playerIsBig = false;
    private bool isGiantCooldown = false;
    private bool giantTimerStart = false;
    private Vector3 targetSize;
    private Rigidbody rb;

    private void TransformToBig()
    {
        if (playerIsBig && transform.localScale != targetSize)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, transformSpeed * Time.deltaTime);

            if (transform.localScale == targetSize && !giantTimerStart)
            {
                giantTimerStart = true;
                StartCoroutine(GiantAbilityDuration());
            }
        }
    }

    private void TransformToNormal()
    {
        if (!playerIsBig && transform.localScale != normalSize)
        {

            if (activeSmokeInstance == null)
            {
                activeSmokeInstance = Instantiate(smokeParticle, smokePos.transform.position, smokePos.transform.rotation, smokePos.transform);
                activeSmokeInstance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            transform.localScale = Vector3.MoveTowards(transform.localScale, normalSize, transformSpeed * Time.deltaTime);
            rb.AddForce(-transform.forward * knockback, ForceMode.Impulse);


            if (transform.localScale == normalSize)
            {
                ParticleSystem ps = activeSmokeInstance.GetComponent<ParticleSystem>();
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                activeSmokeInstance = null;
                Destroy(activeSmokeInstance, 1f);
            }
        }
    }

    private void ToggleChangeSize()
    {
        if (!isGiantCooldown)
        {
            targetSize = new Vector3(1, 1, 1) * sizeMultiplier;

            playerIsBig = true;
        }
    }

    private IEnumerator GiantAbilityDuration()
    {
        yield return new WaitForSeconds(giantDuration);
        StartCoroutine(StartGiantCooldownCoroutine());
    }

    private IEnumerator StartGiantCooldownCoroutine()
    {
        isGiantCooldown = true;
        playerIsBig = false;
        yield return new WaitForSeconds(giantCooldown);
        giantTimerStart = false;
        isGiantCooldown = false;
    }

    private void OnEnable()
    {
        playerInputScript.OnToggleGiant += ToggleChangeSize;
    }

    private void OnDisable()
    {
        playerInputScript.OnToggleGiant -= ToggleChangeSize;
    }
    private void Awake()
    {
        playerInputScript = GetComponent<PlayerInput>();
        normalSize = transform.localScale;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TransformToBig();
        TransformToNormal();
    }
}
