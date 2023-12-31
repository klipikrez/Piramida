using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using static Functions;

public class Side : BaseEnemy
{
    public float startingHealth = 152f;
    public float currentHealth;
    public List<EyeHealthBar> healthBars;
    public GameObject eye;
    public Renderer eyeRenderer;
    [System.NonSerialized]
    public GameObject player;
    public Vector3 playerOffset = Vector3.zero;
    public MeshRenderer shield;
    public GameObject EyeHitbox;
    public SkinnedMeshRenderer kapak;
    private float angle;
    public float rotSpeed = 1f;
    public Quaternion offset;
    Transform defaultRotation;
    Quaternion lookAtRotation;
    public bool lookAtPlayer = true;
    public Vector3 lookAtPoint = Vector3.zero;
    public float blinkState = 0;
    public Transform SIMSshield;
    public float SIMSshieldMaxRotation = 60f;
    public Coroutine SimsCorutine;
    public Bas boss;
    public float ShakeStrenth = 10f;
    public float ShakeSpeed = 50f;
    public float shakeTime = 1f;
    public float ShakeStrenthKill = 10f;
    public float ShakeSpeedKill = 50f;
    public float shakeTimeKill = 1f;
    public VisualEffect gougeEyeVFX;
    public GameObject sjebanoOko;
    public GameObject pushBack;
    public bool headOpen = false;
    public bool dead = false;
    Coroutine flickerCoroutine;
    public ShieldStages shieldStages;
    private void Start()
    {
        defaultRotation = eye.transform;
        currentHealth = startingHealth;
        player = GameObject.FindGameObjectWithTag("Player");

        UpdateMaterial();
    }
    private void Update()
    {
        if (currentHealth != 0)
        {
            Vector3 lookAt = player.transform.position;

            angle = Vector3.Angle((new Vector3(lookAt.x, 0, lookAt.z)) - new Vector3(transform.position.x, 0, transform.position.z), transform.forward);
            if (lookAtPlayer)
            {
                LookAt(lookAt, Mathf.Min(Mathf.Max(180 - angle - 110, 0) * 1.8f, 100) / 100);
                Blink(Mathf.Min(Mathf.Max(180 - angle - 100, 0) * 1.8f, 100));//trepuce
            }
            else
            {
                LookAt(lookAtPoint, 1f);
                Blink(blinkState * 100f);
            }
        }

    }

    public void SetCrazyEyeMode(bool value)
    {
        eyeRenderer.material.SetFloat("_CARZYMODE", value ? 1 : 0);

    }

    public void LookAt(Vector3 lookAt, float weight)
    {
        //Debug.Log(weight);
        Vector3 targetDirection = (lookAt - eye.transform.position).normalized;
        lookAtRotation = Quaternion.Lerp(transform.rotation * offset, Quaternion.LookRotation(targetDirection) * offset, weight);
        eye.transform.rotation = Quaternion.Lerp(eye.transform.rotation, lookAtRotation, DeltaTimeLerp(0.2f));
    }

    public void Blink(float value)
    {
        kapak.SetBlendShapeWeight(0, Mathf.Lerp(value, kapak.GetBlendShapeWeight(0), DeltaTimeLerp(0.8f)));
        kapak.SetBlendShapeWeight(1, Mathf.Lerp(value, kapak.GetBlendShapeWeight(0), DeltaTimeLerp(0.8f)));

    }
    public override void Damage(float damage)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
            UpdateHealthbar();
            boss.ShakeCorutine(UnityEngine.Random.Range(0f, 52f), ShakeStrenth, ShakeSpeed, shakeTime);
        }
        else
        {
            if (!dead)
            {
                dead = true;

                boss.SjebiOsvetljenjeFlicker(0.2f, 252f);
                boss.ShakeCorutine(UnityEngine.Random.Range(0f, 52f), ShakeStrenthKill, ShakeSpeedKill, shakeTimeKill);
                currentHealth = 0;
                UpdateHealthbar();
                AudioManager.Instance.PlayVoiceLine("PiramidaEyeLoss");
                AudioManager.Instance.PlayAudioClip("OdeMuOko");
                gougeEyeVFX.SendEvent("Start");
                eye.SetActive(false);
                kapak.gameObject.SetActive(false);
                EyeHitbox.SetActive(false);
                shield.gameObject.SetActive(false);
                pushBack.SetActive(false);

                sjebanoOko.SetActive(true);
                boss.CheckIfDead();
            }
        }
    }

    private void UpdateHealthbar()
    {
        int tempScaledHealth = Convert.ToInt32((Mathf.Ceil((currentHealth / startingHealth) * healthBars.Count) - 1));

        foreach (EyeHealthBar bar in healthBars)
        {
            if (tempScaledHealth >= 0)
            {
                tempScaledHealth--;
            }
            else
            {
                bar.SetHealth(false);
            }
        }
        UpdateMaterial();
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }
        flickerCoroutine = StartCoroutine(c_Flicker());
    }
    void UpdateMaterial()
    {
        Debug.Log(1 - currentHealth / startingHealth);
        shield.material.SetFloat("_fresnel", shieldStages.fresnel.Evaluate(1 - currentHealth / startingHealth));
        shield.material.SetColor("_Color", shieldStages.color.Evaluate(1 - currentHealth / startingHealth));
        shield.material.SetFloat("_timeSpeed", shieldStages.timeSpeed.Evaluate(1 - currentHealth / startingHealth));
        shield.material.SetFloat("_cellDensity", shieldStages.cellDensity.Evaluate(1 - currentHealth / startingHealth));
        shield.material.SetFloat("_offset", shieldStages.offset.Evaluate(1 - currentHealth / startingHealth));
    }
    IEnumerator c_Flicker()
    {
        float flickerTimer = 0;
        while (flickerTimer < shieldStages.time)
        {
            shield.material.SetFloat("_flicker", shieldStages.kurvaZaFlicker.Evaluate(flickerTimer / shieldStages.time));
            yield return new WaitForEndOfFrame();
            flickerTimer += Time.deltaTime;
        }
        shield.material.SetFloat("_flicker", 0);

    }
    public void SetSimsState(bool value)
    {
        headOpen = value;
        if (SimsCorutine != null)
        {
            StopCoroutine(SimsCorutine);
        }
        SimsCorutine = StartCoroutine(c_Sims(value ? SIMSshieldMaxRotation : 0));
    }

    IEnumerator c_Sims(float targetRotation)
    {

        while (Mathf.Abs(SIMSshield.localRotation.eulerAngles.x - targetRotation) > 1f)
        {
            SIMSshield.localRotation = Quaternion.Lerp(SIMSshield.localRotation, Quaternion.Euler(Vector3.right * targetRotation), DeltaTimeLerp(0.025f));
            yield return new WaitForEndOfFrame();
        }
        SIMSshield.transform.localRotation = Quaternion.Euler(Vector3.right * targetRotation);
    }
}
