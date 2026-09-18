using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class HitPoint : BaseEnemy
{

    //public VisualEffect feathers;
    // Start is called before the first frame update
    private void Start()
    {
        //feathers.SendEvent("Start");
    }
    public override void Damage(float damage)
    {
        //feathers.SendEvent("Start");
        //AudioManager.Instance.PlayAudioClip("PiuPiu", 0.65f);
        Bullet bullet = RopeTomahawk.Instance.T2.GetComponent<Bullet>();
        bullet.transform.position = transform.position;
        ((TomahawkBullet)bullet.bulletBase).StickToWallRadious(bullet, GetComponents<Collider>(), 4);

        // bullet.hitColliders = new List<Collider>();
        //bullet.velocity = Vector3.down;
        //bullet.transform.position = transform.position;
        //Destroy(gameObject);
    }

}
