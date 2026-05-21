using UnityEngine;

public class MeleeEnemyScript : EnemyAI
{
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private Transform attackPoint;

    protected override void Attack(float attackDmg)
    {
        GameObject melee = Instantiate(meleePrefab, attackPoint.position, attackPoint.rotation);

        EnemyMeleeScript meleeScript = melee.GetComponent<EnemyMeleeScript>();
        meleeScript.InitializeEnemyMeleeScript(attackDmg);
    }
}
