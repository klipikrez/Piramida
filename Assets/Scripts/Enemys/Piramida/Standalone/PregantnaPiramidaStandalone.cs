using UnityEngine;

public class PregantnaPiramidaStandalone : BaseStandaloneAttack
{
    [Header("Pregnant pyramid asset")]
    public PregantnaPiramida attack;
    public bool autoStart = true;

    private void Start()
    {
        attackAsset = attack;
        if (autoStart)
        {
            Begin();
        }
    }

    public void Begin()
    {
        attackAsset = attack;
        if (attackAsset == null)
        {
            Debug.LogWarning("PregantnaPiramidaStandalone has no PregantnaPiramida attack asset assigned.");
            return;
        }

        StartAttackAsset();
    }

    private void Update()
    {
        if (attackAsset == null)
        {
            return;
        }

        UpdateAttackAsset();
    }

    public void StopAttack()
    {
        StopAttackAsset();
    }
}
