using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;


[CreateAssetMenu(fileName = "newObelisk", menuName = "Bosses/Piramida/Obelisk")]
public class Obelisk : BaseAttack
{

    public GameObject obeliskPrefab;
    public GameObject onoGovnoIznadGlavePrefab;
    public VisualEffectAsset SummonDone;
    [System.NonSerialized]
    public VisualEffect summonDoneObj;
    public VisualEffectAsset Summon;
    [System.NonSerialized]
    public VisualEffect summonsObj;
    ObeliskAttack attack;
    GameObject heptagram;
    public int attackAmount = 52;
    public float chargeUpTime = 2f;
    public float followAttackTime = 2.5f;
    public float predictAttackTime = 1.5f;
    public float randomAttackTime = 0.9f;
    public float randomAttackRadious = 5f;
    public Vector3 HeptagramSummonOffset = new Vector3(0, 30f, 0);
    int followAttackCounter = 0;
    int randomAttackCounter = 0;
    int predictkAttackCounter = 0;
    float attackDelay;
    float negativeRandom1;
    float negativeRandom2;
    float negativeRandom3;
    bool spawn = false;
    public override void EndAttack(Boss boss)
    {
        EndAttack(AttackContext.FromBoss(boss));
    }

    public override void EndAttack(AttackContext context)
    {
        if (context != null && context.HasPyramidBoss())
        {
            context.SetHeadOpen(false);
            context.SetCrazyEyes(false);
            if (context.boss != null)
            {
                context.boss.ChooseNewRandomState();
            }
        }

        if (summonDoneObj != null)
        {
            Destroy(summonDoneObj.gameObject);
        }
        if (summonsObj != null)
        {
            Destroy(summonsObj.gameObject);
        }
        if (heptagram != null)
        {
            Destroy(heptagram);
        }
    }

    public override void StartAttack(Boss boss)
    {
        StartAttack(AttackContext.FromBoss(boss));
    }

    public override void StartAttack(AttackContext context)
    {
        if (context == null)
        {
            return;
        }

        summonsObj = new GameObject("summon").AddComponent<VisualEffect>();
        summonsObj.visualEffectAsset = Summon;
        summonsObj.transform.position = context.GetOriginPosition() + HeptagramSummonOffset;
        summonsObj.Play();

        if (context.boss != null)
        {
            AudioManager.Instance.PlayAudioClip("ObeliskSummon", 0.65f);
        }

        spawn = false;
        heptagram = Instantiate(onoGovnoIznadGlavePrefab, context.GetOriginPosition() + HeptagramSummonOffset, Quaternion.identity * Quaternion.Euler(180, 0, 0));
        if (heptagram != null)
        {
            HeptagramAboveHead heptagramScript = heptagram.GetComponent<HeptagramAboveHead>();
            if (heptagramScript != null)
            {
                heptagramScript.time = chargeUpTime;
            }
        }

        if (context.HasPyramidBoss())
        {
            context.SetHeadOpen(true);
        }

        followAttackCounter = 0;
        randomAttackCounter = 0;
        predictkAttackCounter = 0;
        negativeRandom1 = -UnityEngine.Random.Range(0f, 1f);
        negativeRandom2 = -UnityEngine.Random.Range(0f, 1f);
        negativeRandom3 = -UnityEngine.Random.Range(0f, 1f);

        if (context.HasPyramidBoss())
        {
            context.SetCrazyEyes(true);
        }
    }

    public override void UpdateAttack(Boss boss)
    {
        UpdateAttack(AttackContext.FromBoss(boss));
    }

    public override void UpdateAttack(AttackContext context)
    {
        if (context == null || context.player == null)
        {
            return;
        }

        if (heptagram != null)
        {
            heptagram.transform.position = context.GetOriginPosition() + HeptagramSummonOffset;
        }
        if (summonsObj != null)
        {
            summonsObj.transform.position = context.GetOriginPosition() + HeptagramSummonOffset;
        }

        if (obeliskPrefab != null)
        {
            attack = obeliskPrefab.GetComponent<ObeliskAttack>();
            if (attack != null)
            {
                attackDelay = attack.attackDelay;
            }
        }

        if (followAttackCounter + randomAttackCounter + predictkAttackCounter > attackAmount)
        {
            EndAttack(context);
            return;
        }

        if (context.timeSinceAttackStarted < chargeUpTime)
        {
            return;
        }

        if (context.timeSinceAttackStarted > chargeUpTime && !spawn)
        {
            spawn = true;
            if (heptagram != null)
            {
                var heptagramScript = heptagram.GetComponent<HeptagramAboveHead>();
                if (heptagramScript != null)
                {
                    heptagramScript.EndWarmup();
                }
            }

            if (context.HasPyramidBoss())
            {
                context.GetPyramidBoss().SjebiOsvetljenjeFlicker(0.15f, 152f);
            }

            summonDoneObj = new GameObject("summon").AddComponent<VisualEffect>();
            summonDoneObj.visualEffectAsset = SummonDone;
            summonDoneObj.transform.position = context.GetOriginPosition() + HeptagramSummonOffset;
            summonDoneObj.Play();
        }

        if (context.timeSinceAttackStarted - chargeUpTime + negativeRandom1 > followAttackTime * followAttackCounter)
        {
            followAttackCounter++;
            SpawnObelisk(context.player.transform.position);
        }

        if (context.timeSinceAttackStarted - chargeUpTime + negativeRandom2 > predictAttackTime * predictkAttackCounter)
        {
            predictkAttackCounter++;
            Vector3 predictedPosition = context.player.transform.position + (context.playerMovement != null ? context.playerMovement.velocity * attackDelay : Vector3.zero);
            SpawnObelisk(predictedPosition);
        }

        if (context.timeSinceAttackStarted - chargeUpTime + negativeRandom3 > randomAttackTime * randomAttackCounter)
        {
            randomAttackCounter++;
            Vector2 vec = UnityEngine.Random.insideUnitCircle;
            SpawnObelisk(context.player.transform.position + new Vector3(vec.x, 0, vec.y) * randomAttackRadious);
        }
    }

    void SpawnObelisk(Vector3 pos)
    {
        if (obeliskPrefab == null)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 25f, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            GameObject a = Instantiate(obeliskPrefab, hit.point, Quaternion.identity);
            ObeliskAttack obeliskAttack = a.GetComponent<ObeliskAttack>();
            if (obeliskAttack != null)
            {
                obeliskAttack.obj.transform.localRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
            }
        }
    }
}
