using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<float> enemyHealthEvent;
    public float enemyMaxHealth;
    public float enemyCurrentHealth;
    [SerializeField] private Slider healthBarSlider;

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
        SetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
