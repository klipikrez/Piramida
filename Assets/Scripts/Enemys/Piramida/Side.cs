using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        defaultRotation = eye.transform;
        currentHealth = startingHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        Vector3 lookAt = player.transform.position + playerOffset;

        angle = Vector3.Angle((new Vector3(lookAt.x, 0, lookAt.z)) - new Vector3(transform.position.x, 0, transform.position.z), transform.forward);
        // Debug.Log(angle);
        LookAt(lookAt, Mathf.Min(Mathf.Max(180 - angle - 110, 0) * 1.8f, 100) / 100);
        kapak.SetBlendShapeWeight(0, Mathf.Min(Mathf.Max(180 - angle - 100, 0) * 1.8f, 100));//trepuce
    }

    public void LookAt(Vector3 lookAt, float weight)
    {
        //Debug.Log(weight);
        Vector3 targetDirection = (lookAt - eye.transform.position).normalized;
        lookAtRotation = Quaternion.Lerp(transform.rotation * offset, Quaternion.LookRotation(targetDirection) * offset, weight);
        eye.transform.rotation = lookAtRotation/*Quaternion.Lerp(lookAtRotation, eye.transform.localRotation, 0.8f)*/;
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
