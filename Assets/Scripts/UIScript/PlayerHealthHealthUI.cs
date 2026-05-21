using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private PlayerHealthScript playerHealthScript;

    public void SetMaxHealthBar(float maxValue)
    {
        healthBar.maxValue = maxValue;
    }

    public void SetHealthBar(float value)
    {
        healthBar.value = value;
    }

    private void OnEnable()
    {
        playerHealthScript.OnHealthUpdate += SetHealthBar;
    }

    private void OnDisable()
    {
        playerHealthScript.OnHealthUpdate -= SetHealthBar;
    }

    private void Awake()
    {
        healthBar = GetComponent<Slider>();

        SetMaxHealthBar(playerHealthScript.MaxHealth);
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
