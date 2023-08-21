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
    public float restTime = 1f;
    public float fartInRestTime = 0.1f;
    public float fartShakeSpeed = 2f;
    public float fartShakeStrenth = 52f;
    bool resting = false;
    int seed = 0;

    public override void EndAttack(Bas boss)
    {
        foreach (Side side in boss.pyramidSides)
        {
            side.lookAtPlayer = true;
            //side.lookAtPoint = new Vector3(0, 152f, 0);
            //side.blinkState = 0.5f;
        }
        boss.returnToNormalRotation = true;
        boss.returnToNormalFloatHeight = true;
        //boss.mainObject.transform.position = new Vector3(0, boss.mainObject.transform.position.y, 0);
        boss.ChooseNewRandomState();
    }

    public override void StartAttack(Bas boss)
    {
        resting = false;
        foreach (Side side in boss.pyramidSides)
        {
            side.lookAtPlayer = false;
            side.lookAtPoint = new Vector3(0, 12f, 0) + side.transform.forward * 6;
            side.blinkState = 0.5f;
        }
        seed = Random.Range(0, 152);
        AudioManager.Instance.PlayAudioClip("PregnantRumble");
        boss.player.ScreenshakeInverse(shakeTime, playerScreenShakeStrenth, playerScreenShakeSpeed);
        boss.returnToNormalFloatHeight = false;
        boss.returnToNormalRotation = false;

    }

    public override void UpdateAttack(Bas boss)
    {
        if (boss.timeSinceAttakStarted < shakeTime)
        {
            Shake(boss, seed, shakeStrenth, shakeSpeed);
            boss.mainObject.transform.position += Vector3.up * Time.deltaTime * 5f;
            foreach (Side side in boss.pyramidSides)
            {
                side.lookAtPoint = side.transform.position + new Vector3(0, 34f, 0) + side.transform.forward * 35f;
                side.blinkState = Functions.Remap(1f - Mathf.PerlinNoise(side.gameObject.GetInstanceID() * 52f, Time.time * blinkingNoiseSpeed), 0, 1, blinkingMin, blinkingMax) * Time.deltaTime * 60f;
            }
        }
        else
        {
            if (boss.timeSinceAttakStarted - shakeTime < restTime)
            {
                if (!resting)
                {
                    AudioManager.Instance.PlayAudioClip("Fart", 0.8f);
                    for (int i = 0; i < spawnNumber; i++)
                    {
                        MiniPiramida instanceMini = Instantiate(miniPiramidaPrefab, boss.mainObject.transform.position + Vector3.one * Random.Range(-1f, 1f), Quaternion.identity).GetComponent<MiniPiramida>();
                        instanceMini.rigidBody.velocity += Vector3.down * 100f;
                        instanceMini.player = boss.player.gameObject.GetComponent<PlayerMovement>();
                    }
                    boss.player.Screenshake(0.5f, playerScreenShakeStrenth * 4, playerScreenShakeSpeed * 4);

                    foreach (Side side in boss.pyramidSides)
                    {
                        side.blinkState = 0.5f;
                    }



                    resting = true;
                }
                else
                {
                    if (boss.timeSinceAttakStarted - shakeTime < fartInRestTime)
                    {
                        Shake(boss, seed + 5, fartShakeStrenth * (1f - (boss.timeSinceAttakStarted - shakeTime) / (restTime / 2 + 0.001f)), fartShakeSpeed);
                    }

                }
            }
            else
            {
                EndAttack(boss);
            }
        }
    }

    void Shake(Bas boss, float seed, float strenth, float speed)
    {
        boss.mainObject.transform.Rotate(Time.deltaTime * 200f * new Vector3(
            (0.4665f - Mathf.PerlinNoise(seed, boss.timeSinceAttakStarted * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 52, boss.timeSinceAttakStarted * speed)) * strenth,
            (0.4665f - Mathf.PerlinNoise(seed + 152, boss.timeSinceAttakStarted * speed)) * strenth));
        boss.mainObject.transform.position += (Time.deltaTime * 200f * new Vector3(
        (0.4665f - Mathf.PerlinNoise(seed, boss.timeSinceAttakStarted * speed)) * strenth,
        (0.4665f - Mathf.PerlinNoise(seed + 52, boss.timeSinceAttakStarted * speed)) * strenth,
        (0.4665f - Mathf.PerlinNoise(seed + 152, boss.timeSinceAttakStarted * speed)) * strenth));
    }



}
