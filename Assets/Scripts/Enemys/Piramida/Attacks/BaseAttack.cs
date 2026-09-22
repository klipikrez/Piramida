using UnityEngine;

public abstract class BaseAttack : ScriptableObject
{
    public int repeatAttack = 1;
    public float chillTimeBeforAttackStarts = 1f;
    public float chillTimeAftrAttackEnds = 2f;

    public virtual void StartAttack(Boss boss)
    {
        StartAttack(AttackContext.FromBoss(boss));
    }

    public virtual void UpdateAttack(Boss boss)
    {
        UpdateAttack(AttackContext.FromBoss(boss));
    }

    public virtual void EndAttack(Boss boss)
    {
        EndAttack(AttackContext.FromBoss(boss));
    }

    public abstract void StartAttack(AttackContext context);
    public abstract void UpdateAttack(AttackContext context);
    public abstract void EndAttack(AttackContext context);
}
