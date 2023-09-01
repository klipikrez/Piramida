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

    public override void EndAttack(Bas boss)
    {
        //        Debug.Log("gotovo");
        Destroy(ShockvaweObject);
        boss.ChooseNewRandomState();
    }

    public override void StartAttack(Bas boss)
    {
        ShockvaweObject = new GameObject("Shockvawe");

        if (playAudio != "" && boss.attackRepeted == 1)
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
        ShockvaweObject.transform.position = boss.transform.position + Vector3.down * 552f;
        ShockvaweObject.transform.rotation = boss.transform.rotation;
        //ShockvaweCollider = Instantiate(new GameObject(), boss.transform.position, boss.transform.rotation).AddComponent<MeshCollider>();
        ShockvaweObject.AddComponent<MeshFilter>().mesh = mesh;
        ShockvaweObject.AddComponent<MeshRenderer>().material = material;

        Keyframe[] tmp = kurvaZaPiramidu.keys;
        tmp[0].value = (/*boss.normalFloatHeight +*/ boss.GroundOffset + boss.mainObject.transform.localPosition.y) / multiplyKurvaPramida;
        //kurvaZaPiramidu.keys[0].value = boss.normalFloatHeight / multiplyKurvaPramida;

        kurvaZaPiramidu.keys = tmp;
    }

    public override void UpdateAttack(Bas boss)
    {

        if (boss.timeSinceAttakStarted < timePiramida)
        {
            boss.returnToNormalFloatHeight = false;
            boss.mainObject.transform.position = /*Vector3.Lerp(*/new Vector3(boss.mainObject.transform.position.x,
                                                             kurvaZaPiramidu.Evaluate(boss.timeSinceAttakStarted / timePiramida) * multiplyKurvaPramida - boss.GroundOffset,
                                                             boss.mainObject.transform.position.z)/*,
                                                             boss.mainObject.transform.position, DeltaTimeLerp(0.95f))*/;
        }
        else
        {
            if (boss.returnToNormalFloatHeight == false)
            {
                AudioManager.Instance.PlayAudioClip("shockvaweHit", 1f);
                boss.player.Screenshake(0.2f, 7f, 45f);
                boss.mainObject.transform.position = new Vector3(
                    boss.mainObject.transform.position.x,
                    -boss.GroundOffset,
                    boss.mainObject.transform.position.z
                );
            }
            boss.returnToNormalFloatHeight = true;
            if (boss.timeSinceAttakStarted - timePiramida < timeShockvawe)
            {
                ShockvaweObject.transform.position = new Vector3(ShockvaweObject.transform.position.x,
                                                                 multiplyKurvaShockvawe * kurvaZaShockvawe.Evaluate((boss.timeSinceAttakStarted - timePiramida) / timeShockvawe) - boss.GroundOffset,
                                                                 ShockvaweObject.transform.position.z);

                ShockvaweObject.transform.localScale = Vector3.one * (boss.timeSinceAttakStarted - timePiramida) * expandSpeed + Vector3.one * startingShockvaweScale;
            }
            else
            {

                EndAttack(boss);
            }
        }
    }
    /*
        public void DamagePlayer(Collider coll)
        {
            PlayerStats player = coll.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.Damage(damage,ga);
            }
        }*/

}
