using UnityEngine;
using UnityEngine.UI;

public class ChargedGrappleBarScript : MonoBehaviour
{
    [SerializeField] private Slider grappleChargedBar;
    [SerializeField] private PlayerGrapple playerGrappleScript;

    public void SetMaxGrappleChargedBar(float maxValue)
    {
        grappleChargedBar.maxValue = maxValue;
    }

    public void SetGrappleChargedBar(float value)
    {
        grappleChargedBar.value = value;
    }

    private void OnEnable()
    {
        playerGrappleScript.OnChargedUpdated += SetGrappleChargedBar;
    }

    private void OnDisable()
    {
        playerGrappleScript.OnChargedUpdated -= SetGrappleChargedBar;
    }

    private void Awake()
    {
        grappleChargedBar = GetComponent<Slider>();

        SetMaxGrappleChargedBar(playerGrappleScript.maximumDischarge);
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
