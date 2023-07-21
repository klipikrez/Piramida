using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTomahawk", menuName = "GunnStuf/Gns/Tomahawk")]
public class Tomahawk : BaseGun
{

    public override void Reload(PlayerArms player)
    {
        if (RopeTomahawk.Instance.T2 != null)
        {
            RopeTomahawk.Instance.reloading = true;
            if (RopeTomahawk.Instance.hit)
            {
                Bullet bullet = BulletManager.Instance.Get();

                bullet.bulletBase = bulletBase;
                //bullet.velocity = Vector3.zero;
                //bullet.gameObject.transform.rotation = player.cam.transform.rotation;
                bullet.gameObject.transform.position = RopeTomahawk.Instance.T2.position;
                bullet.speed = bulletBase.speed;
                bullet.timeAlive = 0f;
                bullet.meshFilter.mesh = bulletBase.mesh;
                bullet.meshFilter.gameObject.transform.localScale = Vector3.one * bulletBase.meshScale;
                Material[] materTiJebem = { bulletBase.materials[0], bulletBase.materials[1] };
                bullet.meshRenderer.materials = materTiJebem;
                bullet.employer = player;
                //GameObject destroy = RopeTomahawk.Instance.T2.gameObject;
                Destroy(RopeTomahawk.Instance.T2.gameObject);
                bullet.gameObject.SetActive(true);
                bullet.Initiate();
                AudioManager.Instance.PlayAudioDDDClipDynamic("spiiin2", bullet.transform, 1f);
            }
        }
        else
        {
            player.reloading = false;
        }
    }


    public override void Shoot(PlayerArms player)
    {
        if (!player.reloading)
        {
            player.ammoPerArm[player.selectedGun] = 0;
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
            AudioManager.Instance.PlayAudioDDDClipDynamic("spiiin2", bullet.transform, 1f);
        }

    }
}