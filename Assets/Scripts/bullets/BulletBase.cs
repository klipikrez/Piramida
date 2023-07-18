using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : ScriptableObject
{
    public float bulletLife;
    public Vector3 hitRadious = Vector3.one;
    public float damage = 1;
    public float speed;
    public Mesh mesh;
    public float meshScale = 1;
    public Material[] materials;
    public abstract void DetectHit(Bullet bullet);
    public abstract void Move(Bullet bullet);
    public abstract void Initiate(Bullet bullet);
}
