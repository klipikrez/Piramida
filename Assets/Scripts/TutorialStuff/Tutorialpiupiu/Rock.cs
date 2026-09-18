using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : BaseEnemy
{
    public Animator animator;
    public override void Damage(float damage)
    {
        animator.Play("fall");
        StartCoroutine(Shake());
        gameObject.layer = LayerMask.GetMask("Default");
    }

    IEnumerator Shake()
    {
        yield return new WaitForSeconds(1.6f);
        PlayerStats.Instance.Screenshake(0.5f, 18f, 115f);
    }
}
