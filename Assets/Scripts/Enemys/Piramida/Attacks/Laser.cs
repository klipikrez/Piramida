using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

[CreateAssetMenu(fileName = "newLaser", menuName = "Bosses/Piramida/Laser")]
public class Laser : BaseAttack
{
    public float laserWarmupTime = 1f;
    public float laserFireTime = 8f;
    public float damage = 50f;
    public float laserWidth = 4;
    [System.NonSerialized]
    public LineRenderer line;
    public Vector3 laserStartOffset = new Vector3(0, 30f, 0);
    public Material laserMaterial;
    public float laserDrag = 3.05f;
    public Vector3 laserVelocity = Vector3.zero;
    [System.NonSerialized]
    public Vector3 taretPos = Vector3.zero;
    public float laserSpeed = 50f;
    [System.NonSerialized]
    public float laserFollowPlayerTimer = 0;
    //public float predictPlayerPosFactor = 1;
    [System.NonSerialized]
    public float Distance = float.MaxValue;
    public Mesh laserHitPointMesh;
    [System.NonSerialized]
    GameObject hitObj;
    [System.NonSerialized]
    bool playSound = true;
    public override void EndAttack(Bas boss)
    {
        Destroy(line);
        Destroy(hitObj);
        boss.ChooseNewRandomState();
    }

    public override void StartAttack(Bas boss)
    {
        Distance = float.MaxValue;
        laserFollowPlayerTimer = 0;
        playSound = true;
        line = boss.gameObject.AddComponent<LineRenderer>();
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;
        line.numCapVertices = 15;
        line.material = laserMaterial;
        line.positionCount = 2;

        taretPos = new Vector3(boss.player.transform.position.x, 0, boss.player.transform.position.z);


        hitObj = new GameObject("LaserHitPoint");

        hitObj.transform.localScale = Vector3.one * laserWidth;

        hitObj.layer = LayerMask.NameToLayer("Attack");

        MeshCollider coll = hitObj.AddComponent<MeshCollider>();
        coll.convex = true;
        coll.isTrigger = true;
        coll.sharedMesh = laserHitPointMesh;

        DamagePlayerOnStayTrigger trigger = hitObj.AddComponent<DamagePlayerOnStayTrigger>();
        trigger.damage = damage;

        hitObj.transform.position = boss.mainObject.transform.position + laserStartOffset;

        hitObj.AddComponent<MeshFilter>().mesh = laserHitPointMesh;
        hitObj.AddComponent<MeshRenderer>().material = laserMaterial;
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
            if (playSound)
            {
                playSound = false;
                AudioManager.Instance.PlayAudioDDDClipDynamic("laser", hitObj.transform, 0.4f);
            }
            if (boss.timeSinceAttakStarted - laserFireTime < laserFireTime)
            {
                Vector3 player = new Vector3(boss.player.transform.position.x, 0, boss.player.transform.position.z);
                Distance = Vector3.Distance(taretPos, player);
                laserFollowPlayerTimer += Time.deltaTime;
                float timeScaledSpeed = laserSpeed * ((Mathf.Pow(9, laserFollowPlayerTimer) - 1f) / 8f) + 0.1f;//funkcija u desmosu imas
                laserVelocity += (player - taretPos).normalized * timeScaledSpeed * Time.deltaTime;
                if (Vector3.Distance(taretPos, player) < Vector3.Distance(taretPos + laserVelocity * Time.deltaTime, player))
                {
                    laserFollowPlayerTimer = 0;
                    laserVelocity -= laserVelocity * laserDrag * Time.deltaTime;
                    laserVelocity += (player - taretPos).normalized * timeScaledSpeed * Time.deltaTime * 10;//ovo ti je da se brze ubrza kad skroz stane laserr

                }
                taretPos += laserVelocity * Time.deltaTime;
                line.SetPosition(line.positionCount - 1, taretPos);
                hitObj.transform.position = taretPos;
                hitObj.transform.rotation = Quaternion.LookRotation((line.GetPosition(0) - taretPos).normalized) * Quaternion.Euler(90, 0, 0);
                hitObj.transform.localScale = new Vector3(hitObj.transform.localScale.x, Vector3.Distance(line.GetPosition(0), hitObj.transform.position), hitObj.transform.localScale.z);

            }
            else
            {
                EndAttack(boss);
            }
        }
    }
}
