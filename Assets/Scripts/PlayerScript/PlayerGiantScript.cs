using System;
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
    [SerializeField] private float giantCamDistanceMultiplier = 1.5f; // tune this down from sizeMultiplier
    [SerializeField] private float giantCamVerticalOffset = 2f;
    [SerializeField] private ParticleSystem gear5SmokeParticle;
    [SerializeField] private float giantParticleRadius = 2.5f;

    private float normalParticleRadius;
    private GameObject activeSmokeInstance;
    private Vector3 normalSize;
    private PlayerInput playerInputScript;
    private bool playerIsBig = false;
    private bool isGiantCooldown = false;
    private bool giantTimerStart = false;
    private Vector3 targetSize;
    private Rigidbody rb;
    public event Action<bool> OnGiantStateChanged;

    private void TransformToBig()
    {
        if (playerIsBig && transform.localScale != targetSize)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, transformSpeed * Time.deltaTime);

            if (transform.localScale == targetSize && !giantTimerStart)
            {
                OnGiantStateChanged?.Invoke(true);
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
                OnGiantStateChanged?.Invoke(true);
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

            ThirdPersonCamera.Instance?.SetCameraDistance(ThirdPersonCamera.Instance.NormalDistance * giantCamDistanceMultiplier);
            ThirdPersonCamera.Instance?.SetCameraVerticalOffset(giantCamVerticalOffset);

            if (gear5SmokeParticle != null)
            {
                var shape = gear5SmokeParticle.shape;
                shape.radius = giantParticleRadius;
            }
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
        ThirdPersonCamera.Instance?.ResetCameraDistance();
        ThirdPersonCamera.Instance?.ResetCameraVerticalOffset();

        if (gear5SmokeParticle != null)
        {
            var shape = gear5SmokeParticle.shape;
            shape.radius = normalParticleRadius;
        }


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
        if (gear5SmokeParticle != null)
            normalParticleRadius = gear5SmokeParticle.shape.radius;
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
