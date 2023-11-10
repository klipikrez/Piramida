using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShield : BaseEnemy
{
    public MeshRenderer rend;
    public ShieldStages stagers;
    public float health = 520f;
    float maxHealth = 25;
    float flickerTimer = 0;
    public AnimationCurve kurvaZaFlicker;
    Coroutine flickerCoroutine;
    private void Start()
    {
        maxHealth = health;
        UpdateMaterial();
    }
    public override void Damage(float damage)
    {
        if (health - damage > 0)
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
            }
            flickerCoroutine = StartCoroutine(c_Flicker());

            health -= damage;
            UpdateMaterial();
        }
        else
        {
            health = 0;
            rend.material.color = Color.white;
        }
    }

    void UpdateMaterial()
    {
        rend.material.SetFloat("_fresnel", stagers.fresnel.Evaluate(1 - health / maxHealth));
        rend.material.SetColor("_Color", stagers.color.Evaluate(1 - health / maxHealth));
        rend.material.SetFloat("_timeSpeed", stagers.timeSpeed.Evaluate(1 - health / maxHealth));
        rend.material.SetFloat("_cellDensity", stagers.cellDensity.Evaluate(1 - health / maxHealth));
        rend.material.SetFloat("_offset", stagers.offset.Evaluate(1 - health / maxHealth));
    }
    IEnumerator c_Flicker()
    {
        flickerTimer = 0;
        while (flickerTimer < stagers.time)
        {
            rend.material.SetFloat("_flicker", stagers.kurvaZaFlicker.Evaluate(flickerTimer / stagers.time));
            yield return new WaitForEndOfFrame();
            flickerTimer += Time.deltaTime;
        }
        rend.material.SetFloat("_flicker", 0);

    }

}