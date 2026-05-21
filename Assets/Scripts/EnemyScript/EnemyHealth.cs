using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<float> enemyHealthEvent;
    public float enemyMaxHealth;
    private float enemyCurrentHealth;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private EnemyData enemyDataObject;

    private void SetMaxHealth()
    {
        enemyCurrentHealth = enemyMaxHealth;
        enemyHealthEvent?.Invoke(enemyMaxHealth);
    }

    public void TakeDamage(float damage)
    {
        enemyCurrentHealth -= damage;
        enemyHealthEvent?.Invoke(enemyCurrentHealth);
    }

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMaxHealth = enemyDataObject.health;

        SetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
