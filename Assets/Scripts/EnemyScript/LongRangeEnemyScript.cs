using System;
using UnityEngine;
using UnityEngine.Animations;

public class LongRangeEnemyScript: EnemyAI
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerPos; // get player rotation


    protected override void Attack(float attackDmg)
    {

        firePoint.LookAt(playerPos);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        EnemyBulletScript bulletScript = bullet.GetComponent<EnemyBulletScript>();
        bulletScript.InitializedEnemyBulletScript(attackDmg);
    }
}
