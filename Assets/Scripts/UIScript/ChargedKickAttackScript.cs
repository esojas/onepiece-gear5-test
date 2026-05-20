using UnityEngine;
using UnityEngine.UI;

public class ChargedKickAttackScript : MonoBehaviour
{
    [SerializeField] private Slider kickChargedBar;
    [SerializeField] private PlayerAttackScript playerAttackScript;

    public void SetMaxKickChargedBar(float maxValue)
    {
        kickChargedBar.maxValue = maxValue;
    }

    public void SetKickChargedBar(float value)
    {
        kickChargedBar.value = value;
    }

    private void OnEnable()
    {
        playerAttackScript.OnKickChargedUpdated += SetKickChargedBar;
    }

    private void OnDisable()
    {
        playerAttackScript.OnKickChargedUpdated -= SetKickChargedBar;
    }

    private void Awake()
    {
        kickChargedBar = GetComponent<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMaxKickChargedBar(playerAttackScript.kickMaximumDischarge());
    }
}
