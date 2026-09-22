using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static Functions;


[CreateAssetMenu(fileName = "newShockvawe", menuName = "Bosses/Piramida/Shockvawe")]
public class Shockvawe : BaseAttack
{
    //public PresetShockvawe presetShockvawe;

    public Mesh mesh;
    public Mesh[] hitboxMeshes;
    public Material material;
    public AnimationCurve kurvaZaShockvawe;
    public AnimationCurve kurvaZaPiramidu;
    public float multiplyKurvaShockvawe = 2;
    public float startingShockvaweScale = 20f;

    public float multiplyKurvaPramida = 2;
    public float damage = 50;
    public float timeShockvawe = 5;
    public float timePiramida = 5;
    public float expandSpeed = 10f;
    [System.NonSerialized]
    public GameObject ShockvaweObject;
    public string playAudio = "";
    public float upForce = 15f;
    public float directionalForce = 90f;

    public override void EndAttack(Boss boss)
    {
        EndAttack(AttackContext.FromBoss(boss));
    }

    public override void EndAttack(AttackContext context)
    {
        if (ShockvaweObject != null)
        {
            Destroy(ShockvaweObject);
        }
        if (context != null && context.HasPyramidBoss() && context.boss != null)
        {
            context.boss.ChooseNewRandomState();
        }
    }

    public override void StartAttack(Boss boss)
    {
        StartAttack(AttackContext.FromBoss(boss));
    }

    public override void StartAttack(AttackContext context)
    {
        ShockvaweObject = new GameObject("Shockvawe");

        if (playAudio != "" && context != null && context.boss != null && context.boss.attackRepeted == 1)
        {
            AudioManager.Instance.PlayVoiceLine(playAudio, 0.5f);
        }


        ShockvaweObject.layer = LayerMask.NameToLayer("Attack");
        foreach (Mesh mesh in hitboxMeshes)
        {
            MeshCollider coll = ShockvaweObject.AddComponent<MeshCollider>();
            coll.convex = true;
            coll.isTrigger = true;
            coll.sharedMesh = mesh;
        }

        DamagePlayerOnEnterTrigger trigger = ShockvaweObject.AddComponent<DamagePlayerOnEnterTrigger>();
        trigger.damage = damage;
        ShockvaweObject.transform.position = context.GetOriginPosition() + Vector3.down * 552f;
        ShockvaweObject.transform.rotation = context.HasPyramidBoss() ? context.boss.transform.rotation : Quaternion.identity;
        ShockvaweObject.AddComponent<MeshFilter>().mesh = mesh;
        ShockvaweObject.AddComponent<MeshRenderer>().material = material;

        PushBack pushBack = ShockvaweObject.AddComponent<PushBack>();
        pushBack.force = directionalForce;
        pushBack.upForce = upForce;

        if (context.HasPyramidBoss())
        {
            Keyframe[] tmp = kurvaZaPiramidu.keys;
            tmp[0].value = (context.GetPyramidBoss().GroundOffset + context.GetPyramidBoss().mainObject.transform.localPosition.y) / multiplyKurvaPramida;
            kurvaZaPiramidu.keys = tmp;
        }
        else
        {
            if (context.player != null)
            {
                context.player.Screenshake(0.2f, 7f, 45f);
            }
            AudioManager.Instance.PlayAudioClip("shockvaweHit", 1f);
        }
    }

    public override void UpdateAttack(Boss boss)
    {
        UpdateAttack(AttackContext.FromBoss(boss));
    }

    public override void UpdateAttack(AttackContext context)
    {
        if (context == null || ShockvaweObject == null)
        {
            return;
        }

        if (context.HasPyramidBoss())
        {
            if (context.timeSinceAttackStarted < timePiramida)
            {
                context.SetReturnToNormalFloatHeight(false);
                context.SetPyramidPosition(new Vector3(context.GetPyramidBoss().mainObject.transform.position.x,
                    kurvaZaPiramidu.Evaluate(context.timeSinceAttackStarted / timePiramida) * multiplyKurvaPramida - context.GetPyramidBoss().GroundOffset,
                    context.GetPyramidBoss().mainObject.transform.position.z));
            }
            else
            {
                if (context.GetPyramidBoss().returnToNormalFloatHeight == false)
                {
                    AudioManager.Instance.PlayAudioClip("shockvaweHit", 1f);
                    if (context.player != null)
                    {
                        context.player.Screenshake(0.2f, 7f, 45f);
                    }
                    context.SetPyramidPosition(new Vector3(
                        context.GetPyramidBoss().mainObject.transform.position.x,
                        -context.GetPyramidBoss().GroundOffset,
                        context.GetPyramidBoss().mainObject.transform.position.z));
                }
                context.SetReturnToNormalFloatHeight(true);
                if (context.timeSinceAttackStarted - timePiramida < timeShockvawe)
                {
                    ShockvaweObject.transform.position = new Vector3(ShockvaweObject.transform.position.x,
                        multiplyKurvaShockvawe * kurvaZaShockvawe.Evaluate((context.timeSinceAttackStarted - timePiramida) / timeShockvawe) - context.GetPyramidBoss().GroundOffset,
                        ShockvaweObject.transform.position.z);
                    ShockvaweObject.transform.localScale = Vector3.one * (context.timeSinceAttackStarted - timePiramida) * expandSpeed + Vector3.one * startingShockvaweScale;
                }
                else
                {
                    EndAttack(context);
                }
            }
        }
        else
        {
            if (context.timeSinceAttackStarted < timeShockvawe)
            {
                ShockvaweObject.transform.position = new Vector3(context.GetOriginPosition().x,
                    multiplyKurvaShockvawe * kurvaZaShockvawe.Evaluate(context.timeSinceAttackStarted / timeShockvawe),
                    context.GetOriginPosition().z);
                ShockvaweObject.transform.localScale = Vector3.one * context.timeSinceAttackStarted * expandSpeed + Vector3.one * startingShockvaweScale;
            }
            else
            {

                EndAttack(context);
            }
        }
    }
}    /*
        public void DamagePlayer(Collider coll)
        {
            PlayerStats player = coll.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.Damage(damage,ga);
            }
        }*/


