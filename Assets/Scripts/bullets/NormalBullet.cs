using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDefaultBullet", menuName = "GunnStuf/Bullet/DefaultBullet")]
public class NormalBullet : BulletBase
{


    public override void DetectHit(Bullet bullet)
    {
        //a nemoo mazdu :(
        if (Physics.CheckBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, ~LayerMask.GetMask("Hitbox", "Player", "Ford", "Mazda")))
        {

            foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("EnemyHitbox")))
            {
                col.gameObject.GetComponentInParent<BaseEnemy>().Damage(bullet.bulletBase.damage);
            }


            BulletManager.Instance.ReurnBulletToPool(bullet);
        }
        //throw new System.NotImplementedException();
    }

    public override void Initiate(Bullet bullet)
    {
        //throw new System.NotImplementedException();
    }

    public override void Move(Bullet bullet)
    {
        bullet.transform.position += bullet.transform.forward * bullet.speed * Time.deltaTime;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
