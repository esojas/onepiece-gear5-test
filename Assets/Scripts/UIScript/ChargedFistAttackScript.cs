using UnityEngine;
using UnityEngine.UI;

public class ChargedFistAttackScript : MonoBehaviour
{
    [SerializeField] private Slider fistChargedBar;
    [SerializeField] private PlayerAttackScript playerAttackScript;

    public void SetMaxFistChargedBar(float maxValue)
    {
        fistChargedBar.maxValue = maxValue;
    }

    public void SetFistChargedBar(float value)
    {
        fistChargedBar.value = value;
    }

    private void OnEnable()
    {
        playerAttackScript.OnFistChargedUpdated += SetFistChargedBar;
    }

    private void OnDisable()
    {
        playerAttackScript.OnFistChargedUpdated -= SetFistChargedBar;
    }

    private void Awake()
    {
        fistChargedBar = GetComponent<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMaxFistChargedBar(playerAttackScript.fistMaximumDischarge());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
