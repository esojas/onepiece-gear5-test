using SmallHedge.SoundManager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public event Action<float> enemyHealthEvent;
    public float enemyMaxHealth;
    private float enemyCurrentHealth;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private EnemyData enemyDataObject;
    [SerializeField] private GameObject enemyDeathVFXPrefab;

    private void SetMaxHealth()
    {
        enemyCurrentHealth = enemyMaxHealth;
        enemyHealthEvent?.Invoke(enemyMaxHealth);
    }

    public void TakeDamage(float damage, Vector3 finalForce)
    {

        enemyCurrentHealth = Mathf.Max(enemyCurrentHealth - damage, 0f);

        StartCoroutine(KnockbackPause(finalForce)); // Because u cant mix navmeshagent and physics so just use navmeshagent

        enemyHealthEvent?.Invoke(enemyCurrentHealth);

        EnemyTakeDamageSound();

        if (enemyCurrentHealth == 0f) Death();
    }

    public void EnemyTakeDamageSound()
    {
        SoundManager.PlaySound(SoundType.Punch, null, .4f);
    }

    private IEnumerator KnockbackPause(Vector3 force)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float elapsed = 0f;
        float duration = 0.3f;

        // normalize to get direction, use magnitude as force strength
        Vector3 direction = force.normalized;
        float strength = force.magnitude;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentStrength = Mathf.Lerp(strength, 0f, elapsed / duration);
            agent.Move(direction * currentStrength * Time.deltaTime);
            yield return null;
        }

    }

    private void Death()
    {
        Instantiate(enemyDeathVFXPrefab, transform.position, Quaternion.identity);


        Destroy(gameObject,0.1f);
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
