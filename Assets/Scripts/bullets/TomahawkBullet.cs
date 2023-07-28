using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Functions;


[CreateAssetMenu(fileName = "newTomahawkBullet", menuName = "GunnStuf/Bullet/TomahawkBullet")]

public class TomahawkBullet : BulletBase
{
    public float rotateSpeed = 365f;
    public GameObject HitTomahawkPrefab;
    public Vector3 startingVelocityAdd = Vector3.zero;
    [System.NonSerialized]
    public bool dead = false;

    public override void DetectHit(Bullet bullet)
    {
        if (!bullet.employer.reloading)
        {
            bool deadZone = false;

            if (Physics.CheckBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("DeadZone")) && !dead)
            {

                foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("DeadZone")))
                {
                    if (col.isTrigger)
                    {




                        //Instantiate(HitTomahawkPrefab, closestColliderPoint, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);
                        bullet.velocity = Vector3.down * 155f;
                        dead = true;
                        //bullet.transform.LookAt(bullet.transform.position + bullet.velocity);
                        if (!bullet.hitColliders.Contains(col))
                        {
                            bullet.hitColliders.Add(col);
                            AudioManager.Instance.PlayAudioClip("hitEnemyCollider", 0.2f);
                        }
                    }
                }
            }
            bool notHitEnemyCollider = true;
            bool notHitEnemy = true;
            RaycastHit hit;


            Collider closestCollider;
            float closestPointDistance = float.MaxValue;
            Vector3 closestColliderPoint = Vector3.zero;
            if (!deadZone && !dead)
            {


                if (Physics.CheckBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, ~LayerMask.GetMask("Hitbox", "Bullet", "Player", "Ford", "Mazda")))
                {

                    foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("EnemyHitbox")))
                    {
                        if (!col.isTrigger)
                        {
                            if (!bullet.hitColliders.Contains(col))
                            {
                                bullet.hitColliders.Add(col);
                                col.gameObject.GetComponentInParent<BaseEnemy>().Damage(bullet.bulletBase.damage);
                                notHitEnemy = false;
                                //bullet.employer.guns[bullet.employer.selectedGun].Reload(bullet.employer);



                                if (!Physics.Raycast(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized, out hit))
                                {
                                    Physics.Raycast(bullet.previousLocation, (closestColliderPoint - bullet.previousLocation).normalized, out hit);
                                }
                                //Instantiate(HitTomahawkPrefab, closestColliderPoint, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);
                                bullet.velocity = Vector3.Lerp(Vector3.Reflect(bullet.velocity.normalized, hit.normal), Quaternion.AngleAxis(45, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized, 0.7f) * Vector3.Magnitude(bullet.velocity);
                                bullet.transform.LookAt(bullet.transform.position + bullet.velocity);
                                AudioManager.Instance.PlayAudioClip("hitEnemy", 0.2f);
                            }
                        }
                    }
                }

                //ovo moz brises
                if (notHitEnemy)
                {



                    foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("EnemyCollider")))
                    {
                        if (!col.isTrigger)
                        {
                            if (!bullet.hitColliders.Contains(col))
                            {
                                bullet.hitColliders.Add(col);

                                //                        Debug.Log(col.name);
                                notHitEnemyCollider = false;
                                //bullet.employer.guns[bullet.employer.selectedGun].Reload(bullet.employer);

                                ///                    Debug.Log(col);
                                float distance = Vector3.Distance(col.ClosestPoint(bullet.transform.position), bullet.transform.position);
                                if (distance < closestPointDistance)
                                {
                                    closestPointDistance = distance;
                                    closestCollider = col;
                                    closestColliderPoint = col.ClosestPoint(bullet.transform.position);
                                }



                                if (!Physics.Raycast(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized, out hit))
                                {
                                    Physics.Raycast(bullet.previousLocation, (closestColliderPoint - bullet.previousLocation).normalized, out hit);
                                }
                                //Instantiate(HitTomahawkPrefab, closestColliderPoint, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);

                                //  Debug.DrawRay(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized, Color.black, 10f);
                                //  Debug.DrawRay(bullet.transform.position, Vector3.left, Color.black, 10f);
                                //  Debug.DrawRay(bullet.transform.position, bullet.velocity.normalized, Color.green, 10f);


                                //                        Debug.Log(bullet.velocity);
                                float angle = Vector3.Angle(bullet.velocity.normalized, hit.normal);
                                //imas ovo u desmos, idi tqamo pa gledaj
                                float loss = (1f - (angle - 90f) / 90f) / 2 + 0.5f;

                                bullet.velocity = Vector3.Reflect(bullet.velocity.normalized, hit.normal) * Vector3.Magnitude(bullet.velocity);
                                bullet.velocity *= loss;
                                // Debug.DrawRay(hit.point, bullet.velocity.normalized, Color.red, 10f);
                                // Debug.Log(bullet.velocity);
                                //bullet.transform.LookAt(bullet.transform.position + bullet.velocity);
                                AudioManager.Instance.PlayAudioClip("hitEnemyCollider", 0.3f);
                            }
                        }
                    }
                }
            }
            if (notHitEnemyCollider && notHitEnemy)
            {
                bool completed = false;
                Collider[] colliders = Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, ~LayerMask.GetMask("Hitbox", "Player", "Bullet", "EnemyHitbox", "EnemyCollider", "Ford", "Mazda"));
                foreach (Collider col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        if (!bullet.hitColliders.Contains(col))
                        {
                            ///                    Debug.Log(col);
                            float distance = Vector3.Distance(col.ClosestPoint(bullet.transform.position), bullet.transform.position);
                            if (distance < closestPointDistance)
                            {
                                closestPointDistance = distance;
                                closestCollider = col;
                                closestColliderPoint = col.ClosestPoint(bullet.transform.position);
                            }
                            completed = true;
                        }
                    }
                }

                if (completed)
                {
                    if (!Physics.Raycast(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized, out hit))
                    {
                        Physics.Raycast(bullet.previousLocation, (closestColliderPoint - bullet.previousLocation).normalized, out hit);
                    }
                    Debug.DrawRay(bullet.transform.position, (closestColliderPoint - bullet.transform.position).normalized);
                    // Debug.Log("name: " + hit.collider.name);
                    RopeTomahawk.Instance.T2 = Instantiate(HitTomahawkPrefab, closestColliderPoint, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation).transform;
                    AudioManager.Instance.PlayAudioClip("hitGround", 0.34f);
                    bullet.velocity = Vector3.zero;
                    RopeTomahawk.Instance.hit = true;
                    BulletManager.Instance.ReurnBulletToPool(bullet);
                }
            }




            /*RopeTomahawk.Instance.T1 = null;
            RopeTomahawk.Instance.T2 = null;*/



        }
        else
        {
            if (Physics.CheckBox(bullet.transform.position, Vector3.one / 2f, bullet.transform.rotation, LayerMask.GetMask("Hitbox")))
            {
                //                Debug.Log("NASO SAM TE PICKO");
                bullet.employer.ammoPerArm[bullet.employer.selectedGun] = 1;
                bullet.employer.movement.grapple = false;
                bullet.employer.reloading = false;
                //bullet.employer.EndAllCorutines();
                RopeTomahawk.Instance.reloading = false;
                RopeTomahawk.Instance.ReleaseT();
                RopeTomahawk.Instance.lineRenderer.positionCount = 0;
                AudioManager.Instance.PlayAudioClip("grabTomahawk", 0.5f);
                BulletManager.Instance.ReurnBulletToPool(bullet);
            }
        }
    }



    public override void Move(Bullet bullet)
    {
        if (!bullet.employer.reloading)
        {
            bullet.meshRenderer.gameObject.transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);

            bullet.transform.position += bullet.velocity * Time.deltaTime;
            //if (bullet.velocity != Vector3.zero)
            bullet.velocity += new Vector3(0, -10f, 0) * Time.deltaTime;
            //bullet.transform.position += new Vector3(0, -10f, 0) * Time.deltaTime;
        }
        else
        {

            RopeTomahawk.Instance.distance = Vector3.Distance(bullet.transform.position, bullet.employer.transform.position);
            bullet.meshRenderer.gameObject.transform.Rotate(Vector3.right * -rotateSpeed * Time.deltaTime);
            bullet.transform.LookAt(bullet.employer.transform.position);

            bullet.velocity += (bullet.employer.transform.position - bullet.transform.position).normalized * bullet.employer.guns[bullet.employer.selectedGun].reloadTime * Time.deltaTime;
            //if (Vector3.Distance(bullet.transform.position, bullet.employer.transform.position) < Vector3.Distance(bullet.transform.position + bullet.velocity * Time.deltaTime, bullet.employer.transform.position))
            //{
            bullet.velocity = Quaternion.AngleAxis(0, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized * bullet.velocity.magnitude;


            //}
            bullet.transform.position += bullet.velocity * Time.deltaTime; //Vector3.Lerp(bullet.transform.position, bullet.employer.transform.position, 1 - Mathf.Pow(1 - (Mathf.Min(bullet.employer.reloadTimer / bullet.employer.guns[bullet.employer.selectedGun].reloadTime, 1f)), Time.deltaTime * 60));

            //bullet.velocity += new Vector3(0, -10f, 0) * Time.deltaTime;
        }
    }

    public override void Initiate(Bullet bullet)
    {
        if (!RopeTomahawk.Instance.hit)
            RopeTomahawk.Instance.ClearPath();
        dead = false;
        RopeTomahawk.Instance.hitTime = 0;
        RopeTomahawk.Instance.hit = false;
        bullet.velocity = bullet.transform.forward * bullet.speed + startingVelocityAdd + bullet.employer.gameObject.GetComponent<PlayerMovement>().velocity;
        RopeTomahawk.Instance.SetT(bullet.employer.transform, bullet.transform);
    }

    public void ReturnToSender()
    {

    }
}
