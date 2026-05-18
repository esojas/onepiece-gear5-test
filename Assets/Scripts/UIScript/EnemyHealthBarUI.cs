using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    private Slider healthBar;
    [SerializeField] private EnemyHealth enemyHealthScript;

    private void OnEnable()
    {
        enemyHealthScript.enemyHealthEvent += UpdateHealthBarUI;
    }

    private void OnDisable()
    {
        enemyHealthScript.enemyHealthEvent -= UpdateHealthBarUI;
    }

    public void SetMaxHealthUI(float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }

    private void UpdateHealthBarUI(float health)
    {
        healthBar.value = health;
        Debug.Log("TakeDamage");
    }

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMaxHealthUI(enemyHealthScript.enemyMaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
