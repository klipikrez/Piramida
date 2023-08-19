using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Functions;

public class MiniPiramida : BaseEnemy
{
    public static int activeAgents = 0;


    [NonSerialized]
    public float rotationTimer = 0f;
    public float spinSpeed = 2f;
    public float spinStrenth = 52f;
    public LayerMask playerMask;
    public LayerMask enviromentCollideMask;
    public Vector3 velocity = Vector3.zero;
    [NonSerialized]
    public PlayerMovement player;
    public float moveSpeed = 5f;
    public float drag = 1f;
    public float speedDamageMultier = 1;
    public float RotationDamageMultiplier = 1;
    public float maxDamage = 52.0025052f;
    public Rigidbody rigidBody;
    public float pushbackForce = 95f;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }



    private void FixedUpdate()
    {
        MoveTowardsPalyer();

        MagicanRotacija(UnityEngine.Random.Range(0, 152));
    }

    public override void Damage(float damage)
    {
        Destroy(gameObject);
    }
    public void TriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {

            PlayerStats playerStats = other.gameObject.GetComponentInParent<PlayerStats>();
            PlayerMovement playerMove = other.gameObject.GetComponentInParent<PlayerMovement>();
            if (playerStats != null && playerMove != null)
            {
                if (playerStats.Damage(Mathf.Min(Mathf.Abs(playerMove.velocity.magnitude - velocity.magnitude) + rigidBody.angularVelocity.magnitude, maxDamage), gameObject))
                {
                    rigidBody.velocity =/* rigidBody.velocity.magnitude*/  (/*(transform.position - playerStats.transform.position).normalized +*/ player.transform.forward).normalized * pushbackForce;
                    rigidBody.angularVelocity = Vector3.zero;
                }
            }
        }
    }



    public void MoveTowardsPalyer()
    {
        if (true)
        {
            Vector3 tmpVelocity = (player.gameObject.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed;

            rigidBody.velocity += tmpVelocity;
            rigidBody.velocity -= velocity * drag * Time.deltaTime;
        }
    }


    private const float magicConstant = 0.75f/* 0.4665f */;
    public void MagicanRotacija(int seed)
    {
        rigidBody.AddTorque(new Vector3(
         magicConstant - Mathf.PerlinNoise(seed, rotationTimer * spinSpeed),
         magicConstant - Mathf.PerlinNoise(seed + 52, rotationTimer * spinSpeed),
         magicConstant - Mathf.PerlinNoise(seed + 152, rotationTimer * spinSpeed)) * spinStrenth * Time.deltaTime);

        rotationTimer += Time.deltaTime;
    }


}
