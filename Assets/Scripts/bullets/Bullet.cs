using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Functions;

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
    public List<Collider> hitColliders; //ne smes da dvaput dilujes damage istom hitbox-u
    public void Initiate()
    {
        hitColliders.Clear();
        bulletBase.Initiate(this);
    }
    void Update()
    {
        if (!GameMenu.Instance.paused)
        {
            bulletBase.DetectHit(this);
            bulletBase.Move(this);
        }

        timeAlive += Time.deltaTime;
        if (timeAlive > bulletBase.bulletLife)
            BulletManager.Instance.ReurnBulletToPool(this);
    }
    /*void OnDrawGizmos()
    {
        Vector3 calculatedPosition = (velocity * Time.deltaTime) + transform.position;

        Box box = CalculateBoxBounds(transform.position, calculatedPosition, bulletBase.hitRadious);
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, box.rotation, transform.lossyScale);
        Gizmos.DrawCube(box.center - transform.position, box.bounds);
    }*/
}
