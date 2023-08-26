using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Functions;

public class Bas : BaseEnemy
{
    public float health = 520;
    [System.NonSerialized]
    public BaseAttack currentAttackState;
    public GameObject mainObject;
    public Side[] pyramidSides;
    public BaseAttack[] attackStates;
    public float timeSinceAttakStarted = 0;
    int selectedAttack = 0;
    int[] avalibeAttacks;
    public PlayerStats player;
    public float normalFloatHeight = 2f;
    [System.NonSerialized]
    public float GroundOffset = 0f;
    public bool returnToNormalFloatHeight = true;
    public bool returnToNormalRotation = true;
    [System.NonSerialized]
    public int attackRepeted = 0;
    public float floatAmplitude = 3f;
    public float floatFrequency = 0.5f;
    [System.NonSerialized]
    public float heightOffset;
    public bool active = true;
    [NonSerialized]
    public Coroutine sjebiOsvetljenjeCorutine = null;
    public Volume NormalVolume;
    public Volume FlashVolume;
    public float rotateSpeed = 2;
    //public float rotateStrenth = 2f;
    private int seed = 0;
    public float minRotateValue = 0;
    public float maxRotateValue = 1;
    Coroutine shakeCorutine;

    private void Start()
    {
        GroundOffset = GetGroundHeihtOffset();
        //mainObject.transform.position = new Vector3(transform.position.x, normalFloatHeight, transform.position.z);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        currentAttackState = attackStates[0];
        attackRepeted++;
        timeSinceAttakStarted = 0;
        currentAttackState.StartAttack(this);
        seed = UnityEngine.Random.Range(0, 152);

        AudioManager.Instance.StartCorutineBre();
    }

    private void Update()
    {
        if (active)
        {
            if (currentAttackState != null)
            {
                currentAttackState.UpdateAttack(this);
                timeSinceAttakStarted += Time.deltaTime;
            }
            if (returnToNormalFloatHeight)
            {
                heightOffset = (Mathf.Sin(timeSinceAttakStarted * floatFrequency) + 1) * floatAmplitude;
                mainObject.transform.position = Vector3.Lerp(
                                                            new Vector3(mainObject.transform.position.x, mainObject.transform.position.y, mainObject.transform.position.z),
                                                            new Vector3(transform.position.x, normalFloatHeight + heightOffset - GroundOffset, transform.position.z),
                                                            DeltaTimeLerp(0.09f));
            }
            if (returnToNormalRotation)
            {
                mainObject.transform.Rotate(new Vector3(0, Functions.Remap(1f - Mathf.PerlinNoise(seed, Time.time * rotateSpeed), 0, 1, minRotateValue, maxRotateValue) * Time.deltaTime * 60f, 0));
                mainObject.transform.rotation = Quaternion.Slerp(mainObject.transform.rotation, Quaternion.Euler(0f, mainObject.transform.eulerAngles.y, 0f), DeltaTimeLerp(0.1f));
            }
        }
    }

    public void ChooseNewRandomState()
    {
        if (currentAttackState.repeatAttack > attackRepeted)
        {
            attackRepeted++;
            timeSinceAttakStarted = 0;
            currentAttackState.StartAttack(this);
        }
        else
        {
            attackRepeted = 1;
            avalibeAttacks = CalculateAvalibeAttacks(selectedAttack);
            int i = UnityEngine.Random.Range(0, attackStates.Length - 1);
            //        Debug.Log(i);
            selectedAttack = avalibeAttacks[i];
            currentAttackState = attackStates[selectedAttack];
            //currentAttackState.transform.position = transform.position;
            //currentAttackState = attackStates[0];
            timeSinceAttakStarted = 0;
            currentAttackState.StartAttack(this);
        }
    }

    public int[] CalculateAvalibeAttacks(int n)
    {
        List<int> tmp = new List<int>();
        for (int i = 0; i < attackStates.Length; i++)
        {
            if (i != n)
            {
                tmp.Add(i);
            }
        }
        return tmp.ToArray();
    }

    /**public float LocalFloatHeightToWorld(float floatHeiht)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 5f, Vector3.down, out hit, LayerMask.GetMask("Ground")))
        {
            return hit.point.y + floatHeiht - 5f;
        }
        return floatHeiht;
    }*/

    public void SetHeadOpen(bool value)
    {
        foreach (Side side in pyramidSides)
        {
            side.SetSimsState(value);
        }
    }

    public float GetGroundHeihtOffset()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 52f, transform.position.z), Vector3.down, out hit, 52f * 2f, LayerMask.GetMask("Ground")))
        {
            return -hit.point.y;
        }
        return 0;

    }


    public void SjebiOsvetljenje(float time = 0)
    {
        if (NormalVolume != null && FlashVolume != null)
        {
            if (sjebiOsvetljenjeCorutine == null)
                sjebiOsvetljenjeCorutine = StartCoroutine(c_SjebiOsvetljenje(time));
        }
    }

    public IEnumerator c_SjebiOsvetljenje(float time = 0)
    {
        NormalVolume.enabled = false;
        FlashVolume.enabled = true;

        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(time);

        NormalVolume.enabled = true;
        FlashVolume.enabled = false;
        sjebiOsvetljenjeCorutine = null;
    }


    public void SjebiOsvetljenjeFlicker(float time = 0, float sineSpeed = 1)
    {
        if (NormalVolume != null && FlashVolume != null)
        {
            if (sjebiOsvetljenjeCorutine == null)
                sjebiOsvetljenjeCorutine = StartCoroutine(c_SjebiOsvetljenjeFlicker(time, sineSpeed));
        }
    }

    public IEnumerator c_SjebiOsvetljenjeFlicker(float time = 0, float sineSpeed = 1)
    {
        float timer = 0;
        while (time > timer)
        {
            if (Mathf.Sin(timer * sineSpeed) > 0)
            {
                NormalVolume.enabled = false;
                FlashVolume.enabled = true;
            }
            else
            {
                NormalVolume.enabled = true;
                FlashVolume.enabled = false;
            }

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }



        NormalVolume.enabled = true;
        FlashVolume.enabled = false;
        sjebiOsvetljenjeCorutine = null;
    }
    public void ShakeCorutine(float seed, float strenth, float speed, float duration)
    {
        if (shakeCorutine != null)
        {
            StopCoroutine(shakeCorutine);
        }
        StartCoroutine(c_Shake(seed, strenth, speed, duration));
    }

    IEnumerator c_Shake(float seed, float strenth, float speed, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            Shake(seed, strenth, speed);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Shake(float seed, float strenth, float speed)
    {
        mainObject.transform.Rotate(Time.deltaTime * 200f * new Vector3(
            (0.4665f - Mathf.PerlinNoise(seed, timeSinceAttakStarted * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 52, timeSinceAttakStarted * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 152, timeSinceAttakStarted * speed)) * strenth));
        mainObject.transform.position += (Time.deltaTime * 200f * new Vector3(
        (0.4665f - Mathf.PerlinNoise(seed, timeSinceAttakStarted * speed)) * strenth,
        (0.4665f - Mathf.PerlinNoise(seed + 52, timeSinceAttakStarted * speed)) * strenth,
        (0.4665f - Mathf.PerlinNoise(seed + 152, timeSinceAttakStarted * speed)) * strenth));
    }

    public override void Damage(float damage)
    {
        health -= damage;
        AudioManager.Instance.PlayVoiceLine("PiramidaHurt");
        currentAttackState.EndAttack(this);
        foreach (Side side in pyramidSides)
        {
            side.Damage(52f);
        }
    }
}
