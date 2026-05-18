using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLightningScript : MonoBehaviour
{
    public event Action holdingLightning;
    public event Action throwLightning;

    PlayerInput playerInput;
    Rigidbody rb;

    [SerializeField] private Transform cam;
    [SerializeField] private float distanceToHoldLightning;
    [SerializeField] private Transform holdLightningPos;
    [SerializeField] private float lightningThrowSpeed;
    [SerializeField] private GameObject lightningRodPrefab;
    [SerializeField] private LayerMask lightning;

    GameObject lightningRod;
    ThunderRodScript thunderRodScript;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerInput.OnPickLightning += CheckIfLightning;
        playerInput.OnThrowLightning += ThrowLightningRodInput;
    }

    private void OnDisable()
    {
        playerInput.OnPickLightning -= CheckIfLightning;
        playerInput.OnThrowLightning -= ThrowLightningRodInput;
    }

    private void CheckIfLightning()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, distanceToHoldLightning, lightning))
        {
            Destroy(hit.collider.gameObject);
            HoldLightningRod();
        }

        Debug.Log("No ligthning detected!");
    }

    private void HoldLightningRod()
    {
        rb.useGravity = false;

        rb.constraints = RigidbodyConstraints.FreezePosition;

        lightningRod = Instantiate(lightningRodPrefab,holdLightningPos);

        holdingLightning.Invoke();
    }

    private void ThrowLightningRodInput()
    {
        throwLightning?.Invoke();

        rb.useGravity = true;

        rb.constraints = RigidbodyConstraints.None;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Destroy(lightningRod);

        Vector3 lightningSpawnPos = holdLightningPos.position;

        Vector3 throwDirection = cam.forward;

        lightningRod = Instantiate(lightningRodPrefab, lightningSpawnPos, Quaternion.identity);

        thunderRodScript = lightningRod.GetComponent<ThunderRodScript>();

        thunderRodScript.InitializedLightningRod(lightningSpawnPos, throwDirection, lightningThrowSpeed);

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
