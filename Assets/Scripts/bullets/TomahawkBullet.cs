using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VFX;
using static Functions;

[CreateAssetMenu(fileName = "newTomahawkBullet", menuName = "GunnStuf/Bullet/TomahawkBullet")]

public class TomahawkBullet : BulletBase
{
    public float rotateSpeed = 365f;
    public GameObject HitTomahawkPrefab;
    public Vector3 startingVelocityAdd = Vector3.zero;
    [System.NonSerialized]
    public bool dead = false;

    public VisualEffectAsset sparkle;


    public override void DetectHit(Bullet bullet)
    {
        if (!bullet.employer.reloading)
        {
            CheckIfDeadZone(bullet);

            //if (!dead)
            //{
            CheckHit(bullet);
            //}
        }
        else
        {
            CheckIfReturnedToPlayer(bullet);
        }
    }

    private void CheckIfDeadZone(Bullet bullet)
    {
        if (Physics.CheckBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("DeadZone")) && !dead)
        {
            foreach (Collider col in Physics.OverlapBox(bullet.transform.position, bullet.bulletBase.hitRadious, bullet.transform.rotation, LayerMask.GetMask("DeadZone")))
            {
                if (col.isTrigger)
                {
                    bullet.velocity = Vector3.down * 80f;
                    dead = true;
                    if (!bullet.hitColliders.Contains(col))
                    {
                        bullet.hitColliders.Add(col);
                        AudioManager.Instance.PlayAudioClip("hitEnemyCollider", 0.2f);
                    }
                }
            }
        }
    }

    private void CheckHit(Bullet bullet)
    {
        Vector3 calculatedPosition = bullet.velocity * Time.deltaTime + bullet.transform.position;
        Box box = CalculateBoxBounds(bullet.transform.position, calculatedPosition, bullet.bulletBase.hitRadious);

        //Hit hit = ReturnClosestHitBoxExclude(bullet.transform.position, bullet.hitColliders, out List<Collider> checkedColliders, bullet.transform.rotation, bullet.bulletBase.hitRadious, LayerMask.GetMask("EnemyHitbox"));
        Hit hit = ReturnClosestHitBoxExclude(box.center, bullet.hitColliders, out List<Collider> checkedColliders, box.rotation, box.bounds, LayerMask.GetMask("EnemyHitbox"));
        if (hit.hit && !dead)
        {
            foreach (Collider col in checkedColliders)
            {
                if (!bullet.hitColliders.Contains(col))
                {
                    bullet.velocity = Vector3.Lerp(Vector3.Reflect(bullet.velocity.normalized, hit.normal), Quaternion.AngleAxis(45, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized, 0.7f) * Vector3.Magnitude(bullet.velocity);
                    bullet.transform.LookAt(bullet.transform.position + bullet.velocity);
                    AudioManager.Instance.PlayAudioClip("hitEnemy", 0.2f);
                    BaseEnemy enemySript = col.gameObject.GetComponentInParent<BaseEnemy>();
                    if (enemySript != null)
                    {
                        Debug.Log(hit.collider.gameObject.tag);
                        if (hit.collider.gameObject.CompareTag("Shield"))
                        {
                            SparkleOnHit(hit.point, hit.normal, hit.collider.gameObject);
                        }
                        col.gameObject.GetComponentInParent<BaseEnemy>().Damage(bullet.bulletBase.damage);
                    }
                    else
                    {
                        if (col.gameObject.transform.parent.transform.parent != null)
                        {
                            enemySript = col.gameObject.transform.parent.GetComponentInParent<BaseEnemy>();
                            if (enemySript != null)
                            {
                                if (hit.collider.gameObject.CompareTag("Shield"))
                                {
                                    SparkleOnHit(hit.point, hit.normal, hit.collider.gameObject.transform.GetChild(0).gameObject);
                                }
                                col.gameObject.GetComponentInParent<BaseEnemy>().Damage(bullet.bulletBase.damage);
                            }
                        }
                    }
                    bullet.hitColliders.Add(col);
                }
            }
            return;
        }


        hit = ReturnClosestHitBoxExclude(box.center, bullet.hitColliders, out checkedColliders, box.rotation, box.bounds, LayerMask.GetMask("EnemyCollider", "ReflectBullet"));
        if (hit.hit)
        {
            foreach (Collider col in checkedColliders)
            {
                if (!bullet.hitColliders.Contains(col))
                {

                    float angle = Vector3.Angle(bullet.velocity.normalized, hit.normal);
                    float loss = (1f - (angle - 90f) / 90f) / 2 + 0.5f;
                    bullet.velocity = Vector3.Reflect(bullet.velocity.normalized, hit.normal) * Vector3.Magnitude(bullet.velocity);
                    bullet.velocity *= loss;
                    AudioManager.Instance.PlayAudioClip("hitEnemyCollider", 0.3f);
                    bullet.hitColliders.Add(col);
                }
            }
            return;
        }

        hit = ReturnClosestHitBoxExclude(box.center, bullet.hitColliders, out checkedColliders, box.rotation, box.bounds, ~LayerMask.GetMask("Hitbox", "Player", "Ignore Raycast", "Bullet", "EnemyHitbox", "EnemyCollider", "Attack", "Ford", "Mazda"));

        if (hit.hit)
        {
            Vector3 fromHit = hit.point - bullet.transform.position;
            float dot = Vector3.Dot(bullet.velocity.normalized, fromHit.normalized);
            if (dot > 0.0f)
            {
                //kad se sekira lupi u zid 
                if (hit.collider.gameObject.CompareTag("RigidbodyInteractable"))
                {
                    bullet.employer.gameObject.GetComponent<PlayerMovement>().hitRigidbody = true;
                    GameObject instance = Instantiate(HitTomahawkPrefab, hit.point, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);
                    Rigidbody rigidBody = hit.collider.gameObject.GetComponent<Rigidbody>();
                    RopeTomahawk.Instance.SetTransformsToFollow(
                    bullet.employer.transform,
                    instance.transform);
                    instance.transform.SetParent(hit.collider.gameObject.transform);
                    rigidBody.AddForceAtPosition(bullet.GetComponent<Bullet>().velocity, hit.point);
                    instance.transform.localScale = Vector3.one * meshScale;
                }
                else
                {

                    bullet.employer.gameObject.GetComponent<PlayerMovement>().hitRigidbody = false;
                    GameObject instance = Instantiate(HitTomahawkPrefab, hit.point, Quaternion.FromToRotation(bullet.transform.up, hit.normal) * bullet.transform.rotation);
                    RopeTomahawk.Instance.SetTransformsToFollow(
                    bullet.employer.transform,
                    instance.transform
                    );
                    instance.transform.localScale = Vector3.one * meshScale;

                }


                AudioManager.Instance.PlayAudioClip("hitGround", 0.34f);
                RopeTomahawk.Instance.hit = true;
                BulletManager.Instance.ReurnBulletToPool(bullet);
            }
        }
    }
    void SparkleOnHit(Vector3 position, Vector3 normal, GameObject obj)
    {

        VisualEffect summonsObj;
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
            renderer = obj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();

        summonsObj = (new GameObject("summon").AddComponent<VisualEffect>());


        summonsObj.visualEffectAsset = sparkle;
        Vector4 color = renderer != null ? renderer.material.GetColor("_Color") : summonsObj.GetVector4("_Color");
        summonsObj.SetVector4("_Color", color);
        summonsObj.transform.position = position;
        summonsObj.gameObject.transform.rotation = Quaternion.FromToRotation(summonsObj.gameObject.transform.up, normal);
        summonsObj.Play();
        summonsObj.SendEvent("Start");
        summonsObj.AddComponent<DestroyAfterTime>().time = 1f;
    }


    private void CheckIfReturnedToPlayer(Bullet bullet)
    {
        if (Physics.CheckBox(bullet.transform.position, Vector3.one / 2f, bullet.transform.rotation, LayerMask.GetMask("Hitbox", "Player")))
        {
            bullet.employer.ammoPerArm[bullet.employer.selectedGun] = 1;
            bullet.employer.movement.grapple = false;
            bullet.employer.reloading = false;
            RopeTomahawk.Instance.reloading = false;
            RopeTomahawk.Instance.ReleaseTransformsToFollow();
            RopeTomahawk.Instance.lineRenderer.positionCount = 0;
            AudioManager.Instance.PlayAudioClip("grabTomahawk", 0.5f);
            BulletManager.Instance.ReurnBulletToPool(bullet);
        }
    }

    public override void Move(Bullet bullet)
    {
        if (!bullet.employer.reloading)
        {
            bullet.meshRenderer.gameObject.transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.right);

            bullet.transform.position += bullet.velocity * Time.deltaTime;
            bullet.velocity += new Vector3(0, -10f, 0) * Time.deltaTime;
        }
        else
        {
            RopeTomahawk.Instance.distance = Vector3.Distance(bullet.transform.position, bullet.employer.transform.position);
            bullet.meshRenderer.gameObject.transform.Rotate(-rotateSpeed * Time.deltaTime * Vector3.right);
            bullet.transform.LookAt(bullet.employer.transform.position);
            bullet.velocity += bullet.employer.guns[bullet.employer.selectedGun].reloadTime * Time.deltaTime * (bullet.employer.transform.position - bullet.transform.position).normalized;
            bullet.velocity = Quaternion.AngleAxis(0, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized * bullet.velocity.magnitude;
            bullet.transform.position += bullet.velocity * Time.deltaTime;
        }
    }

    public override void Initiate(Bullet bullet)
    {
        if (!RopeTomahawk.Instance.hit)
            RopeTomahawk.Instance.ClearPath();
        dead = false;
        RopeTomahawk.Instance.hitTime = 0;
        RopeTomahawk.Instance.bounceTime = 0;
        RopeTomahawk.Instance.hit = false;
        bullet.velocity = bullet.transform.forward * bullet.speed + startingVelocityAdd + bullet.employer.gameObject.GetComponent<PlayerMovement>().velocity;
        RopeTomahawk.Instance.SetTransformsToFollow(bullet.employer.transform, bullet.transform);
    }
}
