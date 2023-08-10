using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

public class Bas : BaseEnemy
{
    [System.NonSerialized]
    public BaseAttack currentAttackState;
    public GameObject mainObject;
    public BaseAttack[] attackStates;
    public float timeSinceAttakStarted = 0;
    int selectedAttack = 0;
    int[] avalibeAttacks;
    public PlayerStats player;
    public float normalFloatHeight = 2f;
    public bool returnToNormalFloatHeight = true;
    [System.NonSerialized]
    public int attackRepeted = 0;
    public float floatAmplitude = 3f;
    public float floatFrequency = 0.5f;
    [System.NonSerialized]
    public float heightOffset;

    private void Start()
    {
        mainObject.transform.position = new Vector3(transform.position.x, normalFloatHeight, transform.position.z);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        currentAttackState = attackStates[0];
        attackRepeted++;
        timeSinceAttakStarted = 0;
        currentAttackState.StartAttack(this);

        AudioManager.Instance.StartCorutineBre();
    }

    private void Update()
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
                                                        mainObject.transform.position,
                                                        new Vector3(mainObject.transform.position.x, normalFloatHeight + heightOffset, mainObject.transform.position.z),
                                                        DeltaTimeLerp(0.14f));
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
            attackRepeted = 0;
            avalibeAttacks = CalculateAvalibeAttacks(selectedAttack);
            int i = Random.Range(0, attackStates.Length - 1);
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

    public override void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
