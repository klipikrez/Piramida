using UnityEngine;

public abstract class BaseStandaloneAttack : MonoBehaviour
{
    [Header("Player reference")]
    public PlayerStats player;
    public bool autoFindPlayer = true;
    public BaseAttack attackAsset;
    protected AttackContext activeContext;

    protected virtual void Reset()
    {
        TryFindPlayer();
    }

    protected virtual void Awake()
    {
        TryFindPlayer();
    }

    protected void TryFindPlayer()
    {
        if (!autoFindPlayer || player != null)
        {
            return;
        }

        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.GetComponent<PlayerStats>();
        }
    }

    protected bool HasPlayer()
    {
        return player != null;
    }

    protected Vector3 GetPlayerPosition()
    {
        if (player == null)
        {
            return transform.position;
        }

        return player.transform.position;
    }

    protected void StartAttackAsset()
    {
        if (attackAsset == null)
        {
            Debug.LogWarning($"{GetType().Name} has no attack asset assigned.");
            return;
        }

        TryFindPlayer();
        activeContext = AttackContext.FromStandalone(transform, player, player != null ? player.GetComponent<PlayerMovement>() : null);
        attackAsset.StartAttack(activeContext);
    }

    protected void UpdateAttackAsset()
    {
        if (attackAsset == null || activeContext == null)
        {
            return;
        }

        activeContext.timeSinceAttackStarted += Time.deltaTime;
        attackAsset.UpdateAttack(activeContext);
    }

    protected void StopAttackAsset()
    {
        if (attackAsset != null && activeContext != null)
        {
            attackAsset.EndAttack(activeContext);
        }
        activeContext = null;
    }
}
