using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bas : BaseEnemy
{
    public BaseAttack currentAttackState;

    public BaseAttack[] attackStates;
    public override void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void ChooseNewRandomState()
    {

    }
}
