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
    public List<Collider> hitColliders; //ne smes da dvaput dilujes damage istom hitbox-u
    public void Initiate()
    {
        hitColliders.Clear();
        bulletBase.Initiate(this);
    }
    void FixedUpdate()
    {
        bulletBase.Move(this);
        bulletBase.DetectHit(this);
        timeAlive += Time.deltaTime;
        if (timeAlive > bulletBase.bulletLife)
            BulletManager.Instance.ReurnBulletToPool(this);
        previousLocation = transform.position;
    }
}
