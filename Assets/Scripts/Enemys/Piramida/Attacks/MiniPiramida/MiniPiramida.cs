using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Functions;

public class MiniPiramida : BaseEnemy
{
    public static List<MiniPiramida> activeAgents = new List<MiniPiramida>();
    public int agentID = 0;

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
    int seed = 0;
    public float sektaDistanceFromPlayer = 7f;
    public float sektaFloatHeight = 7f;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            Debug.LogError("NENENENENEENENENENENEENNENENNNNNNNNNNNNNNNEEEEEEEEEEENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENENEN");
            //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        seed = UnityEngine.Random.Range(0, 152);
        UpdateAgentIDs(true);
    }



    private void FixedUpdate()
    {
        if (activeAgents.Count <= 2)
        {
            MoveTowardsPalyer();

            MagicanRotacija(seed);
        }
        else
        {
            //MoveTowardsPalyer();

            //MagicanRotacija(seed);
            SektaMovement();
        }
    }

    public override void Damage(float damage)
    {
        UpdateAgentIDs(false);
        Destroy(gameObject);
    }
    public void UpdateAgentIDs(bool addNewAgent)
    {
        if (addNewAgent)
        {
            agentID = activeAgents.Count;
            activeAgents.Add(this);
        }
        else
        {
            activeAgents.RemoveAt(agentID);
            for (int i = agentID; i < activeAgents.Count; i++)
            {
                activeAgents[i].agentID = i;
            }
        }
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
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
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
    }



    public void MoveTowardsPalyer()
    {

        Vector3 tmpVelocity = (player.gameObject.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed;

        rigidBody.velocity += tmpVelocity;
        rigidBody.velocity -= velocity * drag * Time.deltaTime;

    }
    public void SektaMovement()
    {
        float angle = agentID * (360f / activeAgents.Count) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * sektaDistanceFromPlayer + Vector3.up * sektaFloatHeight;



        Vector3 targetPosition = player.gameObject.transform.position + offset;


        Vector3 tmpVelocity = (targetPosition - transform.position).normalized * Time.deltaTime * moveSpeed;

        rigidBody.velocity += tmpVelocity;
        rigidBody.velocity -= velocity * drag * Time.deltaTime;
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
