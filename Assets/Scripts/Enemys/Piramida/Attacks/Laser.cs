using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

[CreateAssetMenu(fileName = "newLaser", menuName = "Bosses/Piramida/Laser")]
public class Laser : BaseAttack
{
    public float laserWarmupTime = 1f;
    public float laserFireTime = 8f;
    [System.NonSerialized]
    public LineRenderer line;
    public Vector3 laserStartOffset = new Vector3(0, 30f, 0);
    public Material laserMaterial;
    public float laserLerpValue = 0.05f;
    public Vector3 lerpPostitionToPlayer = Vector3.zero;
    public override void EndAttack(Bas boss)
    {
        Destroy(line);
        boss.ChooseNewRandomState();
    }

    public override void StartAttack(Bas boss)
    {
        lerpPostitionToPlayer =new Vector3(boss.player.transform.position.x,0,boss.player.transform.position.z);
        line = boss.gameObject.AddComponent<LineRenderer>();
        line.startWidth = 5;
        line.endWidth = 5;
        line.numCapVertices = 15;
        line.material = laserMaterial;
        line.positionCount = 2;
    }

    public override void UpdateAttack(Bas boss)
    {
        line.SetPosition(0, boss.mainObject.transform.position + laserStartOffset);
        if (boss.timeSinceAttakStarted < laserWarmupTime)
        {
            line.SetPosition(line.positionCount - 1, boss.mainObject.transform.position + laserStartOffset);
        }
        else
        {
            if (boss.timeSinceAttakStarted - laserFireTime < laserFireTime)
            {
                lerpPostitionToPlayer = Vector3.Lerp(lerpPostitionToPlayer,
                new Vector3(boss.player.transform.position.x, 0, boss.player.transform.position.z),
                DeltaTimeLerp(laserLerpValue));
                line.SetPosition(line.positionCount - 1, lerpPostitionToPlayer);
            }
            else
            {
                EndAttack(boss);
            }
        }
    }
}
