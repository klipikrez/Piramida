using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newTomahawkBullet", menuName = "GunnStuf/Bullet/TomahawkBullet")]

public class TomahawkBullet : BulletBase
{
    public float rotateSpeed = 365f;
    public GameObject HitTomahawkPrefab;
    public bool returnToSender = false;
    public Vector3 startingVelocityAdd = Vector3.zero;



    public override void DetectHit(Bullet bullet)
    {
        bool notHitEnemy = true;
        if (Physics.CheckBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, ~LayerMask.GetMask("Hitbox", "Bullet", "Player", "Ford", "Mazda")))
        {
            Collider closestCollider;
            float closestPointDistance = float.MaxValue;
            Vector3 closestColliderPoint = Vector3.zero;
            foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("EnemyHitbox")))
            {
                col.gameObject.GetComponentInParent<BaseEnemy>().Damage(bullet.bulletBase.damage);
                notHitEnemy = false;
                bullet.employer.guns[bullet.employer.selectedGun].Reload(bullet.employer);

            }

            if (notHitEnemy)
            {
                Collider[] colliders = Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, ~LayerMask.GetMask("Hitbox", "Player", "Bullet", "EnemyHitbox", "Ford", "Mazda"));
                foreach (Collider col in colliders)
                {
                    Debug.Log(col);
                    float distance = Vector3.Distance(col.ClosestPoint(bullet.transform.position), bullet.transform.position);
                    if (distance < closestPointDistance)
                    {
                        closestPointDistance = distance;
                        closestCollider = col;
                        closestColliderPoint = col.ClosestPoint(bullet.transform.position);
                    }
                }
                RaycastHit hit;

                if (!Physics.Raycast(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized, out hit))
                {
                    Physics.Raycast(bullet.previousLocation, (closestColliderPoint - bullet.previousLocation).normalized, out hit);
                }
                Debug.DrawRay(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized);
                Debug.Log("name: " + hit.collider.name);
                Instantiate(HitTomahawkPrefab, closestColliderPoint, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);
            }
            bullet.velocity = Vector3.zero;
            RopeTomahawk.Instance.T1 = null;
            RopeTomahawk.Instance.T2 = null;
            BulletManager.Instance.ReurnBulletToPool(bullet);
        }
    }

    public override void Move(Bullet bullet)
    {


        bullet.meshRenderer.gameObject.transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);

        bullet.transform.position += bullet.velocity * Time.deltaTime;
        if (bullet.velocity != Vector3.zero)
            bullet.velocity += new Vector3(0, -10f, 0) * Time.deltaTime;
        bullet.transform.position += new Vector3(0, -10f, 0) * Time.deltaTime;
    }

    public override void Initiate(Bullet bullet)
    {
        bullet.velocity = bullet.transform.forward * bullet.speed + startingVelocityAdd + bullet.employer.gameObject.GetComponent<PlayerMovement>().velocity / 2f;
        RopeTomahawk.Instance.T1 = bullet.employer.transform;
        RopeTomahawk.Instance.T2 = bullet.transform;
    }

    public void ReturnToSender()
    {

    }
}
