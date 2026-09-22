using UnityEngine;

public class LaserStandalone : BaseStandaloneAttack
{
    [Header("Laser asset")]
    public Laser attack;
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
            Debug.LogWarning("LaserStandalone has no Laser attack asset assigned.");
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
