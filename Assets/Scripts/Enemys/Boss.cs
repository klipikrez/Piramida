using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : BaseEnemy
{
    public PlayerStats player;
    public PlayerMovement playerMovment;
    public BaseAttack currentAttackState;
    public BaseAttack[] attackStates;
    public float ChillBetweenAtacksGlobal = 3f;
    public float timeSinceAttakStarted = 0;

    int selectedAttack = 0;
    int[] avalibeAttacks;
    public int attackRepeted = 0;
    Coroutine waitBetweenAttackCorutine;
    public void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerMovment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentAttackState = attackStates[0];
        attackRepeted++;
        timeSinceAttakStarted = 0;
        currentAttackState.StartAttack(this);
    }

    public void Update()
    {
        if (currentAttackState != null)
        {
            currentAttackState.UpdateAttack(this);
        }
        timeSinceAttakStarted += Time.deltaTime;
    }

    public void ChooseNewRandomState()
    {
        if (waitBetweenAttackCorutine != null)
        {
            StopCoroutine(waitBetweenAttackCorutine);
        }

        if (currentAttackState.repeatAttack > attackRepeted)
        {
            attackRepeted++;
            timeSinceAttakStarted = 0;
            currentAttackState.StartAttack(this);
        }
        else
        {
            BaseAttack previousAttack = currentAttackState;
            currentAttackState = null;
            waitBetweenAttackCorutine = StartCoroutine(c_ChooseNewState(previousAttack));

        }
    }

    IEnumerator c_ChooseNewState(BaseAttack previousAttack)
    {
        attackRepeted = 1;
        avalibeAttacks = CalculateAvalibeAttacks(selectedAttack);
        int i = UnityEngine.Random.Range(0, attackStates.Length - 1);
        selectedAttack = avalibeAttacks[i];
        yield return new WaitForSeconds(previousAttack.chillTimeBeforAttackStarts + attackStates[selectedAttack].chillTimeAftrAttackEnds + ChillBetweenAtacksGlobal);

        //        Debug.Log(i);

        currentAttackState = attackStates[selectedAttack];
        //currentAttackState.transform.position = transform.position;
        //currentAttackState = attackStates[0];
        timeSinceAttakStarted = 0;
        currentAttackState.StartAttack(this);
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
}
