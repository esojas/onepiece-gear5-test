using SmallHedge.SoundManager;
using System;
using UnityEditor;
using UnityEngine;

public class KickProjectile : MonoBehaviour
{
    // Might Change in the future but for now im gonna make it the same as FistProjectile
    [SerializeField] private string enemyLayer;
    private bool movingTowardMaxRange = true;
    private Vector3 maxPositionDestination;
    private Transform playerPositionDestination;
    private float speed;
    private float dmgAmount;
    private MeeleeAttack attackPlayerObject;
    private bool objectIsInitilize = false;
    private float damageBonus;
    private float knockbackAmount;
    private Transform feetTarget;
    private Transform feetMesh;
    public event Action OnKickDestroyed;

    public void InitializeKickProjectile(MeeleeAttack playerAttackData, Vector3 maxPos, Transform playerPos, Transform feetIKTarget, Transform feet, bool isGiant = false)
    {
        attackPlayerObject = playerAttackData;
        maxPositionDestination = maxPos;
        playerPositionDestination = playerPos;
        objectIsInitilize = true;

        dmgAmount = isGiant ? attackPlayerObject.attackGiantSize : attackPlayerObject.attackAmt;

        float distanceTravelled = Vector3.Distance(playerPos.position, maxPos);
        float t = Mathf.InverseLerp(0f, attackPlayerObject.attackRange, distanceTravelled);

        damageBonus = Mathf.Lerp(0f, 25f, t);
        knockbackAmount = Mathf.Lerp(attackPlayerObject.attackKnockback * 4f, attackPlayerObject.attackKnockback * 15f, t);

        feetTarget = feetIKTarget;
        //feetTarget.rotation = Quaternion.Euler(0f, -77.9f, 0f);
        feetMesh = feet;
    }

    private void MoveTowardMaxRange(MeeleeAttack playerAttackData, Vector3 maxPos)
    {
        float speed = playerAttackData.attackSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, maxPos, speed);

        feetTarget.position = transform.position;

        if (transform.position == maxPos)
        {
            movingTowardMaxRange = false;
        }
    }

    private void MoveBackToOrigin(MeeleeAttack playerAttackData, Transform playerPos)
    {
        float speed = playerAttackData.attackSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, playerPos.position, speed);

        feetTarget.position = transform.position;

        if (transform.position == playerPos.position)
        {
            Invoke("Destroy", .05f);
        }
    }

    private void Destroy()
    {
        OnKickDestroyed?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            EnemyHealth enemyHealthScript = other.GetComponent<EnemyHealth>();

            PlayKickHitSound();

            Vector3 direction = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Vector3 finalForce = direction * knockbackAmount;

            enemyHealthScript.TakeDamage(dmgAmount+ damageBonus, finalForce);
        }
    }

    private void PlayKickHitSound()
    {
        SoundManager.PlaySound(SoundType.Kick, null, .4f);
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
        else if (!movingTowardMaxRange && objectIsInitilize)
        {
            MoveBackToOrigin(attackPlayerObject, playerPositionDestination);
        }
    }

    private void LateUpdate()
    {
        if (!objectIsInitilize) return;

        feetMesh.position = transform.position;
    }
}
