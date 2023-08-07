using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float health = 100f;
    public Collider hitbox;
    public float invincibilityTime = 1f;
    public bool canTakeDamage = true;
    public Slider slider;
    public float regen = 25f;
    public int diedTimes = 0;
    public Text text;
    public void Damage(float damage)
    {
        if (canTakeDamage)
        {
            health -= damage;
            AudioManager.Instance.PlayAudioClip("uoh", 1f);
            StartCoroutine("c_InvincibilityFrames");
        }
    }

    private void Update()
    {
        float updatedHealth = health + regen * Time.deltaTime;
        if (updatedHealth > 100 || updatedHealth < 0)
        {
            if (updatedHealth < 0)
            {
                diedTimes++;
                text.text = diedTimes.ToString();
            }
            health = 100;
        }
        else
        {
            health = updatedHealth;
        }
        slider.value = health / 100f;
    }
    public void ContinuousDamage(float damage)
    {
        health -= damage * Time.deltaTime;
        AudioManager.Instance.PlayAudioClip("uoh", 0.4f, 256);
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
