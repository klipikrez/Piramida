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
    public GameObject lookAt;
    public MeshRenderer shield;
    public SkinnedMeshRenderer kapak;
    private float angle;

    private void Start()
    {
        currentHealth = startingHealth;
        lookAt = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        angle = Vector3.Angle((lookAt.transform.position - new Vector3(0, lookAt.transform.position.y, 0)) - transform.position, transform.forward /*- new Vector3(0, transform.forward.y, 0)*/);
        LookAt(lookAt.transform);
        kapak.SetBlendShapeWeight(0, Mathf.Min(Mathf.Max(180 - angle - 100, 0) * 1.8f, 100));//trepuce
    }
    public float rotSpeed = 1f;
    public Quaternion offset;
    Quaternion lookAtRotation;
    public void LookAt(Transform lookAt)
    {
        Vector3 targetDirection = (lookAt.transform.position - eye.transform.position).normalized;
        lookAtRotation = Quaternion.LookRotation(targetDirection) * offset;
        eye.transform.rotation = Quaternion.Lerp(lookAtRotation, eye.transform.rotation, 0.8f);
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
