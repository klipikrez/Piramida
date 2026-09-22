using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShockwaveStandalone : BaseStandaloneAttack
{
    [Header("Shockwave asset")]
    public List<Shockvawe> shockwaveSequence = new List<Shockvawe>();
    public bool autoStart = true;
    public float delayBetweenWaves = 1.5f;

    private float nextWaveTime;
    private int sequenceIndex;
    private int currentRepeatCount;
    private float currentAttackDuration;
    private bool waitingForNextWave;
    private Coroutine BeepCoroutine = null;

    private void Start()
    {
        if (autoStart)
        {
            Begin();
        }
    }

    public void Begin()
    {
        sequenceIndex = 0;
        currentRepeatCount = 1;
        waitingForNextWave = false;
        StartWave();
    }

    private Shockvawe GetWaveAsset()
    {
        if (shockwaveSequence == null || shockwaveSequence.Count == 0)
        {
            return null;
        }

        return shockwaveSequence[sequenceIndex % shockwaveSequence.Count];
    }

    private void StartWave()
    {
        attackAsset = GetWaveAsset();
        if (attackAsset == null)
        {
            Debug.LogWarning("ShockwaveStandalone has no Shockvawe asset assigned.");
            return;
        }

        StartAttackAsset();
        currentAttackDuration = ((Shockvawe)attackAsset).timeShockvawe;
        if (BeepCoroutine != null) StopCoroutine(BeepCoroutine);
        BeepCoroutine = StartCoroutine(c_Beep(3, currentAttackDuration));
    }


    public IEnumerator c_Beep(int times, float attackDuration)
    {
        float beepDivide = times + 1;
        float timeBetween = attackDuration + Mathf.Max(((Shockvawe)attackAsset).chillTimeBeforAttackStarts, ((Shockvawe)attackAsset).chillTimeAftrAttackEnds);
        while (times >= 0)
        {//3 2 1
            yield return new WaitForSeconds(timeBetween / (beepDivide));
            times -= 1;
            AudioManager.Instance.PlayAudioClip("beep"/*, transform.position*/);
        }
    }

    private void Update()
    {

        if (waitingForNextWave)
        {
            if (Time.time >= nextWaveTime)
            {
                waitingForNextWave = false;
                StartWave();
            }
            return;
        }

        if (attackAsset == null)
        {
            return;
        }

        UpdateAttackAsset();

        if (activeContext == null)
        {
            return;
        }

        if (activeContext.timeSinceAttackStarted >= currentAttackDuration)
        {
            StopAttackAsset();

            Shockvawe currentWave = (Shockvawe)attackAsset;
            if (currentRepeatCount < currentWave.repeatAttack)
            {
                currentRepeatCount++;
                waitingForNextWave = true;
                nextWaveTime = Time.time + Mathf.Max(currentWave.chillTimeBeforAttackStarts, currentWave.chillTimeAftrAttackEnds);
                return;
            }

            sequenceIndex = (sequenceIndex + 1) % Mathf.Max(1, shockwaveSequence.Count);
            currentRepeatCount = 1;
            waitingForNextWave = true;

            Shockvawe nextWave = GetWaveAsset();
            float waitTime = Mathf.Max(currentWave.chillTimeBeforAttackStarts, currentWave.chillTimeAftrAttackEnds);
            if (nextWave != null)
            {
                waitTime += nextWave.chillTimeBeforAttackStarts;
            }

            nextWaveTime = Time.time + waitTime;
        }
    }

    public void StopAttack()
    {
        waitingForNextWave = false;
        StopAttackAsset();
    }
}
