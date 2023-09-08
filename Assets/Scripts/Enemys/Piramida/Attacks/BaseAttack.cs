using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttack : ScriptableObject
{
    public int repeatAttack = 1;
    public float chillTimeBeforAttackStarts = 1f;
    public float chillTimeAftrAttackEnds = 2f;
    public abstract void StartAttack(Bas boss);
    public abstract void UpdateAttack(Bas boss);
    public abstract void EndAttack(Bas boss);
}
