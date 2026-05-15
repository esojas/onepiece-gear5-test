using UnityEngine;

[CreateAssetMenu(fileName = "MeeleeAttack", menuName = "Scriptable Objects/MeeleeAttack")]
public class MeeleeAttack : ScriptableObject
{
    public float attackAmt;
    public float attackSpeed;
    public float attackRange;
    public float attackKnockback;
    public float attackCooldown;
    public float attackChargeMultiplier;    
}
