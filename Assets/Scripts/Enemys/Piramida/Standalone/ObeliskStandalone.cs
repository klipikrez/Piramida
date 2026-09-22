using UnityEngine;

public class ObeliskStandalone : BaseStandaloneAttack
{
    [Header("Obelisk asset")]
    public Obelisk attack;
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
            Debug.LogWarning("ObeliskStandalone has no Obelisk attack asset assigned.");
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
