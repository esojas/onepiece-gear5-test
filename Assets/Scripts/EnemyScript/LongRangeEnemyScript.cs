using System;
using UnityEngine;
using UnityEngine.Animations;

public class LongRangeEnemyScript: EnemyAI
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    AnimationScript animationScriptController;
    private float pendingDamage;
    private bool hasPreppedShot = false;


    protected override void TrackPlayer(Transform player)
    {
        Vector3 newPos = player.transform.position + new Vector3(0,1,0);

        if (isAttacking)
        {
            firePoint.LookAt(newPos); 
        }
    }

    protected override void Attack(float attackDmg, AnimationScript animationScript)
    {
        isAttacking = true;
        animationScriptController = animationScript;
        animationScript.ChangeAnimation("enemy_preparingShoot", .1f);
        pendingDamage = attackDmg;

        if (!hasPreppedShot)
        {
            hasPreppedShot = true;
            animationScript.ChangeAnimation("enemy_preparingShoot", 0.1f);
        }
        else
        {
            animationScript.ChangeAnimation("enemy_shooting", 0.1f);
        }
    }


    protected override void OnPlayerLeftRange()
    {
        hasPreppedShot = false;
        isAttacking = false;
    }

    public void SpawnBulletCollider()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        EnemyBulletColliderScript bulletScript = bullet.GetComponent<EnemyBulletColliderScript>();
        bulletScript.InitializedEnemyBulletColliderScript(pendingDamage);
    }
}
