using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletBase bulletBase;
    public float speed = 2;
    public int id;
    public Coroutine bulletLifeCorutine;
    public float timeAlive = 0;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public PlayerArms employer;
    public Vector3 velocity = Vector3.zero;
    public Vector3 previousLocation = Vector3.zero;
    public void Initiate()
    {

        bulletBase.Initiate(this);
    }
    void Update()
    {

        bulletBase.Move(this);
        bulletBase.DetectHit(this);
        timeAlive += Time.deltaTime;
        if (timeAlive > bulletBase.bulletLife)
            BulletManager.Instance.ReurnBulletToPool(this);
        previousLocation = transform.position;
    }



    /* public void Initiate()
     {
         bulletLifeCorutine = StartCoroutine(die());
     }

     IEnumerator die()
     {


         yield return new WaitForSeconds(bulletBase.bulletLife);
         if (this.isActiveAndEnabled)



     }*/
}
