using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
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
    [System.NonSerialized]
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
    bool attackStart = true;
    public LayerMask laserCanPassTrough;
    public Vector3 addRotation;
    public GameObject fire;
    public float fireSpawnTime = 0.1f;
    private float fireTimer = 0;
    public VisualEffectAsset lightning;
    public VisualEffectAsset energyOrbes;
    [System.NonSerialized]
    public VisualEffect energyOrbesObj;
    [System.NonSerialized]
    public VisualEffect lightningObj;
    public float maxHitDistance = 252f;
    [System.NonSerialized]
    public Transform hitPoint;
    [System.NonSerialized]
    private GameObject laserOwner;
    public override void EndAttack(Boss boss)
    {
        EndAttack(AttackContext.FromBoss(boss));
    }

    public override void EndAttack(AttackContext context)
    {
        if (lightningObj != null)
        {
            Destroy(lightningObj.gameObject);
        }
        if (hitPoint != null)
        {
            Destroy(hitPoint.gameObject);
        }
        if (line != null)
        {
            Destroy(line.gameObject);
        }
        if (laserOwner != null)
        {
            Destroy(laserOwner);
        }
        if (hitObj != null)
        {
            Destroy(hitObj);
        }

        if (context != null && context.HasPyramidBoss())
        {
            context.SetHeadOpen(false);
            context.SetCrazyEyes(false);
            if (context.boss != null)
            {
                context.boss.ChooseNewRandomState();
            }
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

        if (context.HasPyramidBoss())
        {
            context.SetHeadOpen(true);
        }

        laserVelocity = Vector3.zero;
        Distance = float.MaxValue;
        laserFollowPlayerTimer = 0;
        attackStart = true;

        laserOwner = new GameObject("LaserOwner");
        if (context.HasPyramidBoss() && context.GetPyramidBoss().mainObject != null)
        {
            laserOwner.transform.SetParent(context.GetPyramidBoss().mainObject.transform, false);
        }

        line = laserOwner.AddComponent<LineRenderer>();
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;
        line.numCapVertices = 15;
        line.material = laserMaterial;
        line.positionCount = 2;

        taretPos = new Vector3(context.GetPlayerPosition().x, 0f, context.GetPlayerPosition().z);

        hitObj = new GameObject("Laser");
        hitPoint = new GameObject("LaserHitPoint").transform;
        hitPoint.position = taretPos;
        hitObj.transform.localScale = Vector3.zero;
        hitObj.layer = LayerMask.NameToLayer("Attack");

        MeshCollider coll = hitObj.AddComponent<MeshCollider>();
        coll.convex = true;
        coll.isTrigger = true;
        coll.sharedMesh = laserHitPointMesh;

        DamagePlayerOnStayTrigger trigger = hitObj.AddComponent<DamagePlayerOnStayTrigger>();
        trigger.damage = damage;

        hitObj.transform.position = context.GetOriginPosition() + laserStartOffset;
        hitObj.AddComponent<MeshFilter>().mesh = laserHitPointMesh;
        hitObj.AddComponent<MeshRenderer>().material = laserMaterial;

        if (context.boss != null)
        {
            AudioManager.Instance.PlayAudioClip("LaserWarmup", 0.6f);
        }

        energyOrbesObj = new GameObject("LaserEnergyOrbes").AddComponent<VisualEffect>();
        energyOrbesObj.visualEffectAsset = energyOrbes;
        energyOrbesObj.transform.position = context.GetOriginPosition() + laserStartOffset;
        energyOrbesObj.Play();
    }

    public override void UpdateAttack(Boss boss)
    {
        UpdateAttack(AttackContext.FromBoss(boss));
    }

    public override void UpdateAttack(AttackContext context)
    {
        if (context == null || context.player == null || line == null)
        {
            return;
        }

        line.SetPosition(0, context.GetOriginPosition() + laserStartOffset);
        if (context.timeSinceAttackStarted < laserWarmupTime)
        {
            line.SetPosition(line.positionCount - 1, context.GetOriginPosition() + laserStartOffset);
            if (energyOrbesObj != null)
            {
                energyOrbesObj.transform.LookAt(context.player.transform.position);
                energyOrbesObj.transform.position = context.GetOriginPosition() + laserStartOffset;
            }
        }
        else
        {
            Vector3 start = context.GetOriginPosition() + laserStartOffset;
            if (attackStart)
            {
                if (context.HasPyramidBoss())
                {
                    context.SetCrazyEyes(true);
                }
                if (context.boss != null && context.boss.player != null)
                {
                    context.boss.player.ScreenshakeSource(3f, 65f, hitPoint, 5f);
                }
                attackStart = false;
                if (context.boss != null)
                {
                    AudioManager.Instance.PlayAudioDDDClipDynamic("laser", hitObj.transform, 0.7f);
                }
                lightningObj = new GameObject("LaserLightning").AddComponent<VisualEffect>();
                lightningObj.visualEffectAsset = lightning;
                lightningObj.transform.position = context.GetOriginPosition() + laserStartOffset;
                lightningObj.Play();
                if (energyOrbesObj != null)
                {
                    Destroy(energyOrbesObj.gameObject);
                }
                if (context.HasPyramidBoss())
                {
                    context.GetPyramidBoss().SjebiOsvetljenjeFlicker(0.15f, 152f);
                }
            }

            if (context.timeSinceAttackStarted - laserFireTime < laserFireTime)
            {
                Vector3 player = context.player.transform.position;
                Distance = Vector3.Distance(taretPos, player);
                laserFollowPlayerTimer += Time.deltaTime;
                float timeScaledSpeed = laserSpeed * ((Mathf.Pow(9, laserFollowPlayerTimer) - 1f) / 8f) + 0.1f;
                laserVelocity += (player - taretPos).normalized * timeScaledSpeed * Time.deltaTime;
                if (Vector3.Distance(taretPos, player) < Vector3.Distance(taretPos + laserVelocity * Time.deltaTime, player))
                {
                    laserFollowPlayerTimer = 0;
                    laserVelocity -= laserVelocity * laserDrag * Time.deltaTime;
                    laserVelocity += (player - taretPos).normalized * timeScaledSpeed * Time.deltaTime * 10f;
                }
                taretPos += laserVelocity * Time.deltaTime;

                RaycastHit hit;
                float distance = maxHitDistance;
                fireTimer += Time.deltaTime;

                if (Physics.Raycast(context.GetOriginPosition() + laserStartOffset, (taretPos - start).normalized, out hit, maxHitDistance, laserCanPassTrough))
                {
                    distance = Vector3.Distance(context.GetOriginPosition() + laserStartOffset, hit.point);
                    line.SetPosition(line.positionCount - 1, hit.point);
                    if (hitPoint != null)
                    {
                        hitPoint.position = hit.point;
                    }

                    if (fireTimer * laserVelocity.magnitude > fireSpawnTime)
                    {
                        if (!hit.collider.gameObject.CompareTag("DontLeaveLaserTrail"))
                        {
                            fireTimer = 0;
                            Instantiate(fire, hit.point + hit.normal * 0.3f, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0));
                        }
                    }
                }
                else
                {
                    line.SetPosition(line.positionCount - 1, (taretPos - start).normalized * maxHitDistance + start);
                    if (hitPoint != null)
                    {
                        hitPoint.position = (taretPos - start).normalized * maxHitDistance + start;
                    }
                }

                if (hitObj != null)
                {
                    hitObj.transform.position = context.GetOriginPosition() + laserStartOffset;
                    hitObj.transform.rotation = Quaternion.LookRotation((start - taretPos).normalized) * Quaternion.Euler(addRotation);
                    hitObj.transform.localScale = new Vector3(laserWidth, distance + 4f, laserWidth);
                }
            }
            else
            {
                EndAttack(context);
            }
        }
    }
}
