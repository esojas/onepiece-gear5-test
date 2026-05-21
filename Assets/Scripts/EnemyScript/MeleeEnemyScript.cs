using UnityEngine;

public class MeleeEnemyScript : EnemyAI
{
    protected override void Attack(float dmgAmount)
    {
        Debug.Log($"MeleeAttack {dmgAmount}");
    }
}
