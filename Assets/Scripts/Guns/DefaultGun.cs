using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newDefaultGun", menuName = "GunnStuf/Gns/DefaultGinn")]
public class DefaultGun : BaseGun
{
    public override void Reload(PlayerArms player)
    {
        // throw new System.NotImplementedException();
    }

    public override void ShiftCancelled(PlayerArms player)
    {
        // throw new System.NotImplementedException();
    }

    public override void ReloadCancelled(PlayerArms player)
    {
        // throw new System.NotImplementedException();
    }

    public override void Shift(PlayerArms player)
    {
        // throw new System.NotImplementedException();
    }

    public override void Shoot(PlayerArms player)
    {
        Bullet bullet = BulletManager.Instance.Get();

        bullet.bulletBase = bulletBase;
        bullet.gameObject.transform.rotation = player.cam.transform.rotation;
        bullet.gameObject.transform.position = player.transform.position + bullet.gameObject.transform.TransformDirection(spawnLocation);
        bullet.speed = bulletBase.speed;
        bullet.timeAlive = 0;
        bullet.meshFilter.mesh = bulletBase.mesh;
        bullet.meshFilter.gameObject.transform.localScale = Vector3.one * bulletBase.meshScale;
        bullet.employer = player;
        bullet.meshRenderer.material = bulletBase.materials[0];
        bullet.gameObject.SetActive(true);
        bullet.Initiate();


    }

    public override string ToString()
    {
        return base.ToString();
    }
}
