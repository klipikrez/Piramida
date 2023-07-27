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
    int selectedAttack = -1;
    int[] avalibeAttacks;
    public PlayerStats player;
    public float normalFloatHeight = 2f;
    public bool returnToNormalFloatHeight = true;
    public int attackRepeted = 0;

    private void Start()
    {
        mainObject.transform.position = new Vector3(transform.position.x, normalFloatHeight, transform.position.z);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        currentAttackState = attackStates[0];
        ChooseNewRandomState();
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
            mainObject.transform.position = Vector3.Lerp(mainObject.transform.position, new Vector3(mainObject.transform.position.x, normalFloatHeight, mainObject.transform.position.z), DeltaTimeLerp(0.14f));
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
            avalibeAttacks = CalculateAvalibeAttacks(selectedAttack);
            foreach (int m in avalibeAttacks)
            {
                Debug.Log(m);
            }
            int i = Random.Range(0, attackStates.Length - 1);
            Debug.Log("--0- " + i);
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
