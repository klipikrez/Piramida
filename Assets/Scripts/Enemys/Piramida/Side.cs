using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

public class Side : BaseEnemy
{
    public float startingHealth = 152f;
    public float currentHealth;
    public List<EyeHealthBar> healthBars;
    public GameObject eye;
    [System.NonSerialized]
    public GameObject player;
    public Vector3 playerOffset = Vector3.zero;
    public MeshRenderer shield;
    public SkinnedMeshRenderer kapak;
    private float angle;
    public float rotSpeed = 1f;
    public Quaternion offset;
    Transform defaultRotation;
    Quaternion lookAtRotation;
    public bool lookAtPlayer = true;
    public Vector3 lookAtPoint = Vector3.zero;
    public float blinkState = 0;
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

    public void LookAt(Vector3 lookAt, float weight)
    {
        //Debug.Log(weight);
        Vector3 targetDirection = (lookAt - eye.transform.position).normalized;
        lookAtRotation = Quaternion.Lerp(transform.rotation * offset, Quaternion.LookRotation(targetDirection) * offset, weight);
        eye.transform.rotation = Quaternion.Lerp(eye.transform.rotation, lookAtRotation, DeltaTimeLerp(0.1f));
    }

    public void Blink(float value)
    {
        kapak.SetBlendShapeWeight(0, Mathf.Lerp(value, kapak.GetBlendShapeWeight(0), DeltaTimeLerp(0.8f)));
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
            currentHealth = 0;
            UpdateHealthbar();
            shield.gameObject.SetActive(false);
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
}
