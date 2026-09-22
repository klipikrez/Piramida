using UnityEngine;

public class AttackContext
{
    public Boss boss;
    public PiramidaBoss pyramidBoss;
    public PlayerStats player;
    public PlayerMovement playerMovement;
    public Transform origin;
    public Vector3 originOffset = Vector3.zero;
    public float timeSinceAttackStarted;
    public bool isStandalone;

    public static AttackContext FromBoss(Boss boss)
    {
        var context = new AttackContext
        {
            boss = boss,
            pyramidBoss = boss as PiramidaBoss,
            player = boss != null ? boss.player : null,
            playerMovement = boss != null ? boss.playerMovment : null,
            timeSinceAttackStarted = boss != null ? boss.timeSinceAttakStarted : 0f,
            isStandalone = false,
        };

        if (context.pyramidBoss != null && context.pyramidBoss.mainObject != null)
        {
            context.origin = context.pyramidBoss.mainObject.transform;
        }

        return context;
    }

    public static AttackContext FromStandalone(Transform origin, PlayerStats player, PlayerMovement playerMovement = null, Vector3? offset = null)
    {
        return new AttackContext
        {
            origin = origin,
            player = player,
            playerMovement = playerMovement,
            timeSinceAttackStarted = 0f,
            isStandalone = true,
            originOffset = offset ?? Vector3.zero,
        };
    }

    public Vector3 GetOriginPosition()
    {
        if (origin != null)
        {
            return origin.position + originOffset;
        }

        if (pyramidBoss != null && pyramidBoss.mainObject != null)
        {
            return pyramidBoss.mainObject.transform.position + originOffset;
        }

        return Vector3.zero;
    }

    public Vector3 GetPlayerPosition()
    {
        if (player != null)
        {
            return player.transform.position;
        }

        return Vector3.zero;
    }

    public PiramidaBoss GetPyramidBoss()
    {
        return pyramidBoss;
    }

    public bool HasPyramidBoss()
    {
        return pyramidBoss != null;
    }

    public void SetHeadOpen(bool value)
    {
        if (pyramidBoss != null)
        {
            pyramidBoss.SetHeadOpen(value);
        }
    }

    public void SetCrazyEyes(bool value)
    {
        if (pyramidBoss == null)
        {
            return;
        }

        foreach (Side side in pyramidBoss.pyramidSides)
        {
            if (side != null)
            {
                side.SetCrazyEyeMode(value);
            }
        }
    }

    public void ShakePyramid(float seed, float strenth, float speed)
    {
        if (pyramidBoss == null)
        {
            return;
        }

        pyramidBoss.Shake(seed, strenth, speed);
    }

    public void SetPyramidPosition(Vector3 pos)
    {
        if (pyramidBoss != null && pyramidBoss.mainObject != null)
        {
            pyramidBoss.mainObject.transform.position = Vector3.Lerp(pyramidBoss.mainObject.transform.position, pos, Time.deltaTime * 60);
        }
    }

    public void SetReturnToNormalFloatHeight(bool value)
    {
        if (pyramidBoss != null)
        {
            pyramidBoss.returnToNormalFloatHeight = value;
        }
    }

    public void SetReturnToNormalRotation(bool value)
    {
        if (pyramidBoss != null)
        {
            pyramidBoss.returnToNormalRotation = value;
        }
    }
}
