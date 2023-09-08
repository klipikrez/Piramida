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
    public override void EndAttack(Bas boss)
    {
        boss.SetHeadOpen(false);
        Destroy(summonDoneObj.gameObject);
        Destroy(summonsObj.gameObject);
        Destroy(heptagram);
        foreach (Side side in boss.pyramidSides)
        {
            side.SetCrazyEyeMode(false);
        }
        boss.ChooseNewRandomState();

    }

    public override void StartAttack(Bas boss)
    {
        summonsObj = new GameObject("summon").AddComponent<VisualEffect>();
        summonsObj.visualEffectAsset = Summon;
        summonsObj.transform.position = boss.mainObject.transform.position + HeptagramSummonOffset;
        summonsObj.Play();

        AudioManager.Instance.PlayAudioClip("ObeliskSummon", 0.65f);
        spawn = false;
        heptagram = Instantiate(onoGovnoIznadGlavePrefab, boss.transform.position + HeptagramSummonOffset, boss.transform.rotation * Quaternion.Euler(180, 0, 0));
        heptagram.GetComponent<HeptagramAboveHead>().time = chargeUpTime;
        boss.SetHeadOpen(true);
        followAttackCounter = 0;
        randomAttackCounter = 0;
        predictkAttackCounter = 0;
        negativeRandom1 = -UnityEngine.Random.Range(0f, 1f);
        negativeRandom2 = -UnityEngine.Random.Range(0f, 1f);
        negativeRandom3 = -UnityEngine.Random.Range(0f, 1f);
        //        Debug.Log(negativeRandom1 + " " +
        //negativeRandom2 + " " + negativeRandom3 + " ");
        foreach (Side side in boss.pyramidSides)
        {
            side.SetCrazyEyeMode(true);
        }
    }

    public override void UpdateAttack(Bas boss)
    {
        heptagram.transform.position = boss.mainObject.transform.position + HeptagramSummonOffset;
        summonsObj.transform.position = boss.mainObject.transform.position + HeptagramSummonOffset;
        attack = obeliskPrefab.GetComponent<ObeliskAttack>();
        attackDelay = attack.attackDelay;
        if (followAttackCounter + randomAttackCounter + predictkAttackCounter > attackAmount)
        {
            EndAttack(boss);
        }

        if (boss.timeSinceAttakStarted < chargeUpTime)
        {
            return;
        }

        if (boss.timeSinceAttakStarted > chargeUpTime)
        {
            if (!spawn)
            {
                spawn = true;
                heptagram.GetComponent<HeptagramAboveHead>().EndWarmup();
                boss.SjebiOsvetljenjeFlicker(0.15f, 152f);
                summonDoneObj = new GameObject("summon").AddComponent<VisualEffect>();
                summonDoneObj.visualEffectAsset = SummonDone;
                summonDoneObj.transform.position = boss.mainObject.transform.position + HeptagramSummonOffset;
                summonDoneObj.Play();
            }
        }

        if (boss.timeSinceAttakStarted - chargeUpTime + negativeRandom1 > followAttackTime * followAttackCounter)
        {
            //Debug.Log("Follow: " + followAttackCounter);
            followAttackCounter++;
            SpawnObelisk(boss.player.transform.position);
        }


        if (boss.timeSinceAttakStarted - chargeUpTime + negativeRandom2 > predictAttackTime * predictkAttackCounter)
        {
            //            Debug.Log("predict: " + predictkAttackCounter);
            predictkAttackCounter++;
            SpawnObelisk(boss.playerMovment.velocity * attackDelay + boss.player.transform.position);
        }

        if (boss.timeSinceAttakStarted - chargeUpTime + negativeRandom3 > randomAttackTime * randomAttackCounter)
        {
            // Debug.Log("Random:" + randomAttackCounter);
            randomAttackCounter++;
            Vector2 vec = UnityEngine.Random.insideUnitCircle;

            SpawnObelisk(boss.player.transform.position + new Vector3(vec.x, 0, vec.y) * randomAttackRadious);
        }

    }

    void SpawnObelisk(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 25f, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            GameObject a = Instantiate(obeliskPrefab, hit.point, quaternion.Euler(hit.normal));
            a.GetComponent<ObeliskAttack>().obj.transform.localRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
        }
    }
}
