using System.Collections;
using System.Collections.Generic;
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
    public override void EndAttack(Bas boss)
    {
        Destroy(lightningObj.gameObject);
        Destroy(hitPoint.gameObject);
        Destroy(line);
        Destroy(hitObj);
        boss.ChooseNewRandomState();
    }

    public override void StartAttack(Bas boss)
    {
        laserVelocity = Vector3.zero;
        Distance = float.MaxValue;
        laserFollowPlayerTimer = 0;
        attackStart = true;
        line = boss.gameObject.AddComponent<LineRenderer>();
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;
        line.numCapVertices = 15;
        line.material = laserMaterial;
        line.positionCount = 2;

        taretPos = new Vector3(boss.player.transform.position.x, 0, boss.player.transform.position.z);


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

        hitObj.transform.position = boss.mainObject.transform.position + laserStartOffset;

        hitObj.AddComponent<MeshFilter>().mesh = laserHitPointMesh;
        hitObj.AddComponent<MeshRenderer>().material = laserMaterial;

        AudioManager.Instance.PlayAudioClip("LaserWarmup", 0.6f);


        energyOrbesObj = new GameObject("LaserEnergyOrbes").AddComponent<VisualEffect>();
        energyOrbesObj.visualEffectAsset = energyOrbes;
        energyOrbesObj.transform.position = boss.mainObject.transform.position + laserStartOffset;
        energyOrbesObj.Play();

    }

    public override void UpdateAttack(Bas boss)
    {
        line.SetPosition(0, boss.mainObject.transform.position + laserStartOffset);
        if (boss.timeSinceAttakStarted < laserWarmupTime)
        {
            line.SetPosition(line.positionCount - 1, boss.mainObject.transform.position + laserStartOffset);
            energyOrbesObj.transform.LookAt(boss.player.transform.position);
            energyOrbesObj.transform.position = boss.mainObject.transform.position + laserStartOffset;
        }
        else
        {
            Vector3 start = boss.mainObject.transform.position + laserStartOffset;
            if (attackStart)
            {
                boss.player.ScreenshakeSource(3f, 65f, hitPoint, 5f);
                attackStart = false;
                AudioManager.Instance.PlayAudioDDDClipDynamic("laser", hitObj.transform, 0.7f);
                lightningObj = new GameObject("LaserLightning").AddComponent<VisualEffect>();
                lightningObj.visualEffectAsset = lightning;
                lightningObj.transform.position = boss.mainObject.transform.position + laserStartOffset;
                lightningObj.Play();
                Destroy(energyOrbesObj.gameObject);
                boss.SjebiOsvetljenjeFlicker(0.15f, 252f);

            }
            if (boss.timeSinceAttakStarted - laserFireTime < laserFireTime)
            {
                Vector3 player = boss.player.transform.position;
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
                //taretPos = new Vector3(taretPos.x, boss.player.transform.position.y, taretPos.z);
                RaycastHit hit;
                float distance = maxHitDistance;

                fireTimer += Time.deltaTime;

                if (Physics.Raycast(boss.mainObject.transform.position + laserStartOffset, (taretPos - start).normalized, out hit, maxHitDistance, laserCanPassTrough))
                {
                    distance = Vector3.Distance((boss.mainObject.transform.position + laserStartOffset), hit.point);
                    line.SetPosition(line.positionCount - 1, hit.point);
                    hitPoint.position = hit.point;

                    if (fireTimer * laserVelocity.magnitude > fireSpawnTime)
                    {
                        fireTimer = 0;
                        Instantiate(fire, hit.point + hit.normal * 0.3f, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0));
                    }

                }
                else
                {
                    line.SetPosition(line.positionCount - 1, (taretPos - start).normalized * maxHitDistance + start);
                    hitPoint.position = (taretPos - start).normalized * maxHitDistance + start;
                }

                Debug.DrawRay(boss.mainObject.transform.position + laserStartOffset,
                 distance * (taretPos - start).normalized,
                  Color.black,
                   0.1f);

                //hitObj.transform.position = (taretPos - (boss.mainObject.transform.position + laserStartOffset)) / 2 + (boss.mainObject.transform.position + laserStartOffset);
                hitObj.transform.position = boss.mainObject.transform.position + laserStartOffset;
                hitObj.transform.rotation = Quaternion.LookRotation((start - taretPos).normalized) * Quaternion.Euler(addRotation);
                hitObj.transform.localScale = new Vector3(laserWidth, distance + 4f, laserWidth);




            }
            else
            {
                EndAttack(boss);
            }
        }
    }
}
