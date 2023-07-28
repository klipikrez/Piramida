using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float health = 100f;
    public Collider hitbox;
    public float invincibilityTime = 1f;
    public bool canTakeDamage = true;
    public void Damage(float damage)
    {
        if (canTakeDamage)
        {
            health -= damage;
            AudioManager.Instance.PlayAudioClip("uoh", 1f);
            StartCoroutine("c_InvincibilityFrames");
        }
    }

    public void ContinuousDamage(float damage)
    {
        health -= damage * Time.deltaTime;
        AudioManager.Instance.PlayAudioClip("uoh", 0.4f);
        //AudioManager.Instance.PlayAudioClip("uoh", 1f);
        //StartCoroutine("c_InvincibilityFrames");
    }

    public IEnumerator c_InvincibilityFrames()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invincibilityTime);
        canTakeDamage = true;
    }
}
