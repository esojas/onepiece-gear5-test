using UnityEngine;

public class MeleeEnemyScript : EnemyAI
{
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private Transform attackPoint;
    private float pendingDamage;
    protected override void Attack(float attackDmg, AnimationScript animationScript)
    {
        isAttacking = true;
        animationScript.ChangeAnimation("enemy_swordAttack", .1f);
        pendingDamage = attackDmg;
}

    public void SpawnMeleeCollider()
    {
        GameObject melee = Instantiate(meleePrefab, attackPoint.position, attackPoint.rotation);
        EnemyMeleeColliderScript meleeScript = melee.GetComponent<EnemyMeleeColliderScript>();
        meleeScript.InitializeEnemyMeleeColliderScript(pendingDamage);
    }

}
