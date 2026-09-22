using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPregantnaPiramada", menuName = "Bosses/Piramida/PregantnaPirmida")]
public class PregantnaPiramida : BaseAttack
{
    public float shakeTime = 2f;
    public float shakeSpeed = 2f;
    public float shakeStrenth = 52f;
    public float blinkingNoiseSpeed = 5;
    public float blinkingMin = 0;
    public float blinkingMax = 1;
    public GameObject miniPiramidaPrefab;
    public float playerScreenShakeSpeed = 2f;
    public float playerScreenShakeStrenth = 2f;
    public int spawnNumber = 4;
    public int numberAfterWitchWeSubtract = 3;
    public int NumberSubtract = 2;
    public float restTime = 1f;
    public float fartInRestTime = 0.1f;
    public float fartShakeSpeed = 2f;
    public float fartShakeStrenth = 52f;
    bool resting = false;
    int seed = 0;


    public override void EndAttack(Boss boss)
    {
        EndAttack(AttackContext.FromBoss(boss));
    }

    public override void EndAttack(AttackContext context)
    {
        if (context == null)
        {
            return;
        }

        if (context.HasPyramidBoss())
        {
            foreach (Side side in context.GetPyramidBoss().pyramidSides)
            {
                side.lookAtPlayer = true;
            }
            context.SetReturnToNormalRotation(true);
            context.SetReturnToNormalFloatHeight(true);
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

        resting = false;
        seed = Random.Range(0, 152);

        if (context.HasPyramidBoss())
        {
            foreach (Side side in context.GetPyramidBoss().pyramidSides)
            {
                side.lookAtPlayer = false;
                side.lookAtPoint = new Vector3(0, 12f, 0) + side.transform.forward * 6;
                side.blinkState = 0.5f;
            }
            context.SetReturnToNormalFloatHeight(false);
            context.SetReturnToNormalRotation(false);
        }

        if (context.boss != null && context.boss.player != null)
        {
            AudioManager.Instance.PlayAudioClip("PregnantRumble");
            context.boss.player.ScreenshakeInverse(shakeTime, playerScreenShakeStrenth, playerScreenShakeSpeed);
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

        if (context.timeSinceAttackStarted < shakeTime)
        {
            if (context.HasPyramidBoss())
            {
                context.ShakePyramid(seed, shakeStrenth, shakeSpeed);
                context.SetPyramidPosition(context.GetPyramidBoss().mainObject.transform.position + Vector3.up * Time.deltaTime * 5f);
                foreach (Side side in context.GetPyramidBoss().pyramidSides)
                {
                    side.lookAtPoint = side.transform.position + new Vector3(0, 34f, 0) + side.transform.forward * 35f;
                    side.blinkState = Functions.Remap(1f - Mathf.PerlinNoise(side.gameObject.GetInstanceID() * 52f, Time.time * blinkingNoiseSpeed), 0, 1, blinkingMin, blinkingMax) * Time.deltaTime * 60f;
                }
            }
        }
        else
        {
            if (context.timeSinceAttackStarted - shakeTime < restTime)
            {
                if (!resting)
                {
                    if (context.boss != null)
                    {
                        AudioManager.Instance.PlayAudioClip("Fart", 0.8f);
                    }

                    Vector3 spawnCenter = context.GetOriginPosition();
                    if (context.HasPyramidBoss())
                    {
                        spawnCenter = context.GetPyramidBoss().mainObject.transform.position;
                    }

                    for (int i = 0; i < spawnNumber - ((MiniPiramida.activeAgents.Count > numberAfterWitchWeSubtract) ? NumberSubtract : 0); i++)
                    {
                        MiniPiramida instanceMini = Instantiate(miniPiramidaPrefab, spawnCenter + Vector3.one * Random.Range(-1f, 1f), Quaternion.identity).GetComponent<MiniPiramida>();
                        if (instanceMini != null)
                        {
                            instanceMini.rigidBody.velocity += Vector3.down * 100f;
                            instanceMini.player = context.player.gameObject.GetComponent<PlayerMovement>();
                        }
                    }
                    context.player.Screenshake(0.5f, playerScreenShakeStrenth * 4, playerScreenShakeSpeed * 4);

                    if (context.HasPyramidBoss())
                    {
                        foreach (Side side in context.GetPyramidBoss().pyramidSides)
                        {
                            side.blinkState = 0.5f;
                        }
                    }

                    resting = true;
                }
                else if (context.timeSinceAttackStarted - shakeTime < fartInRestTime && context.HasPyramidBoss())
                {
                    context.ShakePyramid(seed + 5, fartShakeStrenth * (1f - (context.timeSinceAttackStarted - shakeTime) / (restTime / 2 + 0.001f)), fartShakeSpeed);
                }
            }
            else
            {
                EndAttack(context);
            }
        }
    }
}
