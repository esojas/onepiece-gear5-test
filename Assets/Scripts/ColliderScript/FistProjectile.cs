using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class FistProjectile : MonoBehaviour
{
    private bool movingTowardMaxRange = true;
    private Vector3 maxPositionDestination;
    private Transform playerPositionDestination;
    private float speed;
    private float dmgAmount;
    private MeeleeAttack attackPlayerObject;
    private bool objectIsInitilize = false;

    public void InitializeFistProjectile(MeeleeAttack playerAttackData, Vector3 maxPos, Transform playerPos)
    {
        attackPlayerObject = playerAttackData;
        maxPositionDestination = maxPos;
        playerPositionDestination = playerPos;
        objectIsInitilize = true;
        dmgAmount = attackPlayerObject.attackAmt;
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
        EnemyHealth enemyHealthScript = other.GetComponent<EnemyHealth>();

        enemyHealthScript.TakeDamage(dmgAmount);

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
