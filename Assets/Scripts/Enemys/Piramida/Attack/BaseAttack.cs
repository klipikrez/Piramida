using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttack : ScriptableObject
{
    public abstract void Attack(GameObject player, GameObject attacker);
}
