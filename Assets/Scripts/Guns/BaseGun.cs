using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : ScriptableObject
{
    //public int maxBullets;
    //public float reloadTime;
    public float BPS;
    //public float bulletLife;
    public float maxAmmo;
    public float reloadTime;
    public BulletBase bulletBase;
    public Vector3 spawnLocation = new Vector3(0, 1, 0);
    public abstract void Shoot(PlayerArms player);
    public abstract void Reload(PlayerArms player);
    public abstract void ReloadCancelled(PlayerArms player);
    public abstract void Shift(PlayerArms player);
    public abstract void ShiftCancelled(PlayerArms player);

}
