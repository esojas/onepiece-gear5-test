using System;
using UnityEngine;
using UnityEngine.Animations;

public class LongRangeEnemyScript: EnemyAI
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerPos; // get player rotation
    AnimationScript animationScriptController;
    private float pendingDamage;
    private bool hasPreppedShot = false;


    protected override void TrackPlayer()
    {
        if (isAttacking)
        {
            firePoint.LookAt(playerPos); 
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
