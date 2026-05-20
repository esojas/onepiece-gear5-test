using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class FistProjectile : MonoBehaviour
{
    [SerializeField] private string enemyLayer;
    private bool movingTowardMaxRange = true;
    private Vector3 maxPositionDestination;
    private Transform playerPositionDestination;
    private float speed;
    private float dmgAmount;
    private MeeleeAttack attackPlayerObject;
    private bool objectIsInitilize = false;
    private float damageBonus;

    public void InitializeFistProjectile(MeeleeAttack playerAttackData, Vector3 maxPos, Transform playerPos)
    {
        attackPlayerObject = playerAttackData;
        maxPositionDestination = maxPos;
        playerPositionDestination = playerPos;
        objectIsInitilize = true;
        dmgAmount = attackPlayerObject.attackAmt;

        float distanceTravelled = Vector3.Distance(playerPos.position, maxPos);

        // Get a 0-1 value of how far through the max range it is
        float t = Mathf.InverseLerp(0f, attackPlayerObject.attackRange, distanceTravelled);

        // Map that to a 0-25 bonus
        damageBonus = Mathf.Lerp(0f, 25f, t);

        //Debug.LogError(damageBonus);
    }

    private void MoveTowardMaxRange(MeeleeAttack playerAttackData, Vector3 maxPos)
    {
        float speed = playerAttackData.attackSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, maxPos, speed);

        if (transform.position == maxPos)
        {
            movingTowardMaxRange = false;
        }
    }

    private void MoveBackToOrigin(MeeleeAttack playerAttackData, Transform playerPos)
    {
        float speed = playerAttackData.attackSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, playerPos.position, speed);

        if (transform.position == playerPos.position)
        {
            Invoke("Destroy", .05f);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            EnemyHealth enemyHealthScript = other.GetComponent<EnemyHealth>();

            Vector3 direction = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

            Vector3 upForce = new Vector3(0, 2, 0); // for now ill set it like this

            Vector3 finalDirection = direction + upForce;

            other.attachedRigidbody.AddForce(finalDirection * attackPlayerObject.attackKnockback, ForceMode.Impulse);

            enemyHealthScript.TakeDamage(dmgAmount + damageBonus);

            Debug.LogWarning(dmgAmount + damageBonus);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTowardMaxRange && objectIsInitilize)
        {
            MoveTowardMaxRange(attackPlayerObject, maxPositionDestination);
        }
        else if(!movingTowardMaxRange && objectIsInitilize)
        {
            MoveBackToOrigin(attackPlayerObject, playerPositionDestination);
        }
    }
}
