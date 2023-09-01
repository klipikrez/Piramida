using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


[CreateAssetMenu(fileName = "newObelisk", menuName = "Bosses/Piramida/Obelisk")]
public class Obelisk : BaseAttack
{

    public GameObject obeliskPrefab;
    public int attackAmount = 52;
    public float chargeUpTime = 2f;
    public float timeBetweenAttacks = 0.5f;
    int obeliskAttackCounter = 0;
    public override void EndAttack(Bas boss)
    {
        throw new System.NotImplementedException();
    }

    public override void StartAttack(Bas boss)
    {
        obeliskAttackCounter = 0;
    }

    public override void UpdateAttack(Bas boss)
    {
        if (obeliskAttackCounter > attackAmount)
        {
            EndAttack(boss);
        }

        if (boss.timeSinceAttakStarted < chargeUpTime)
        {
            return;
        }

        if (boss.timeSinceAttakStarted - chargeUpTime > timeBetweenAttacks * obeliskAttackCounter)
        {
            obeliskAttackCounter++;
            RaycastHit hit;
            if (Physics.Raycast(boss.player.transform.position, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
            {
                Instantiate(obeliskPrefab, hit.point, quaternion.Euler(hit.normal));
            }
        }

    }
}
