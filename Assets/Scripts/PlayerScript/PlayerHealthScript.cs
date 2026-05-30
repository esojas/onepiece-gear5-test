using SmallHedge.SoundManager;
using System;
using UnityEngine;

public class PlayerHealthScript : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    [SerializeField] private GameObject deathVFX;
    public event Action<float> OnHealthUpdate;
    private float currentHealth;

    private void SetMaxHealth()
    {
        currentHealth = maxHealth;
        OnHealthUpdate?.Invoke(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        PlayerTakeDamageSound();

        OnHealthUpdate?.Invoke(currentHealth);

        if (currentHealth == 0f) Die();
    }

    public void PlayerTakeDamageSound()
    {
        SoundManager.PlaySound(SoundType.Hurt, null,.4f);
    }

    private void Die()
    {
        Instantiate(deathVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
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
