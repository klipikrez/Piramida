using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTomahawk", menuName = "GunnStuf/Gns/Tomahawk")]
public class Tomahawk : BaseGun
{

    public override void Reload(PlayerArms player)
    {
        //return true;
    }


    public override void Shoot(PlayerArms player)
    {
        Bullet bullet = BulletManager.Instance.Get();

        bullet.bulletBase = bulletBase;
        //bullet.velocity = Vector3.zero;
        bullet.gameObject.transform.rotation = player.cam.transform.rotation;
        bullet.gameObject.transform.position = player.cam.transform.position + bullet.gameObject.transform.TransformDirection(spawnLocation);
        bullet.speed = bulletBase.speed;
        bullet.timeAlive = 0f;
        bullet.meshFilter.mesh = bulletBase.mesh;
        bullet.meshFilter.gameObject.transform.localScale = Vector3.one * bulletBase.meshScale;
        Material[] materTiJebem = { bulletBase.materials[0], bulletBase.materials[1] };
        bullet.meshRenderer.materials = materTiJebem;
        bullet.employer = player;
        bullet.gameObject.SetActive(true);
        bullet.Initiate();
    }
}
