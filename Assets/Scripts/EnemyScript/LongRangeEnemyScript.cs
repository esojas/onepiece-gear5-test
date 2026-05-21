using UnityEngine;

public class LongRangeEnemyScript: EnemyAI
{
    protected override void Attack(float dmgAmount)
    {
        Debug.Log($"LongRangeAttack {dmgAmount}");
    }
}
