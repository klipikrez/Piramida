using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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
    private void Start()
    {
        defaultRotation = eye.transform;
        currentHealth = startingHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
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
        }
        else
        {
            if (currentHealth != 0)
            {
                currentHealth = 0;
                UpdateHealthbar();
                AudioManager.Instance.PlayAudioClip("PiramidaEyeLoss");
                EyeHitbox.SetActive(false);
                shield.gameObject.SetActive(false);
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
    }

    public void SetSimsState(bool value)
    {
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
