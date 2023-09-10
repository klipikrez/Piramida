using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Functions;

public class PlayerMovement : MonoBehaviour
{
    [System.NonSerialized]
    public Rigidbody body;
    [System.NonSerialized]
    public PlayerInput input;
    [System.NonSerialized]
    public Camera PlayerCamera;
    public float mouseSensetivity = 1f;
    public float moveSpeed = 1f;
    public float moveSpeedInGrapple = 20f;
    public float moveSpeedInGrappleAir = 10;
    [System.NonSerialized]
    public bool grounded = true;
    [System.NonSerialized]
    public bool jump = false;
    [System.NonSerialized]
    public bool initiatedJumpByPlayer = false;//ovo ti je kad skocis i pustis dugme za skakanje, da program zna da josuvek nisi zavrsio skok
    public float jumpForce = 1f;
    public float jumpPush = 20f;
    public float jumpSpeedBoostMultiply = 2f;
    public Vector3 velocity = Vector3.zero;
    public float maxSpeed = 2;
    public float maxSpeedInJump = 2;
    public float maxSpeedDampenArea = 1;
    public float maxSpeedInGrapple = 10f;
    //private float camRotationX = 0;
    public float stoppingDrag = 0.8f;
    public float stoppingDragGrapple = 0.9f;
    public float normalWalkAngle = 45f;
    public float maxWalkAngle = 60f;
    public LayerMask excludePlayer;
    public float groundCheckRadious = 0.3f;
    public float groundCheckoffset = 0.3f - 0.02f;
    public float groundCheckoffsetForRaycast = 3f;
    [System.NonSerialized]
    public bool grapple = false;
    [System.NonSerialized]
    public bool hitRigidbody = false;
    [System.NonSerialized]
    public bool wasGrappling = false;
    public float grappleForce = 10f;
    public float upPushInGrapple = 18f;
    [System.NonSerialized]
    public float grappleTimer = 0f;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (body == null)
        {
            body = gameObject.GetComponent<Rigidbody>();
        }
        if (input == null)
        {
            input = new PlayerInput();
            input.Player.Enable();
        }
        if (PlayerCamera == null)
        {
            PlayerCamera = gameObject.GetComponentInChildren<Camera>();
        }
        input.Player.Jump.performed += JumpStart;
        input.Player.Jump.canceled += JumpEnd;
        input.Player.Move.canceled += StopMove;

        input.Player.Shift.performed += Shift;
        input.Player.Shift.canceled += StopShift;
    }


    private void Shift(InputAction.CallbackContext context)
    {

    }

    private void StopShift(InputAction.CallbackContext context)
    {

    }

    private void StopMove(InputAction.CallbackContext context)
    {
        //ovo ti je kad ides ides ides na uzbrdo, i ond stanes odjednom, da ne poskocis malo na gore zbog ubrzanja, ovo ti podeli ubrzanje da stanes odma.
        if (grounded && body.velocity.y > 0)
        {
            Hit hit = ReturnClosestHitSphere(transform.position + Vector3.up * groundCheckoffsetForRaycast, groundCheckRadious, ~excludePlayer);
            if (hit.hit)
            {
                body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);//body.velocity * ((1 - (Vector3.Angle(hit.normal, Vector3.down) - 90) / 90));
                //                Debug.Log(((1 - (Vector3.Angle(hit.normal, Vector3.down) - 90) / 90)));
            }
        }
    }

    private void JumpStart(InputAction.CallbackContext context)
    {
        //ako bi tvoji drugari skocili u dunav, dal bi i ti?
        jump = true;
        if (grounded && !grapple && body.velocity.magnitude < maxSpeed)
        {
            body.velocity = new Vector3(body.velocity.x * jumpSpeedBoostMultiply, body.velocity.y, body.velocity.z * jumpSpeedBoostMultiply);
        }
    }

    private void JumpEnd(InputAction.CallbackContext context)
    {
        //a sto si skocio u dunav breee :(
        jump = false;
        if (body.velocity.y > 0 && !grapple)
        {
            body.velocity = new Vector3(body.velocity.x, body.velocity.y / 1.8f, body.velocity.z);
        }
    }

    void Update()
    {

        grounded = Physics.CheckSphere(transform.position + new Vector3(0, groundCheckoffset, 0), groundCheckRadious, ~excludePlayer);
        if (grounded)
        {
            wasGrappling = false;
        }

        if (grapple && !hitRigidbody)
        {
            wasGrappling = true;
            Grapple();

            grappleTimer += Time.deltaTime;
        }
        else
        {
            grappleTimer = 0;
            Move();
            Jump();
        }


        //        Debug.Log(grappleTimer);
        Look();

        velocity = body.velocity;
    }

    public void Grapple()
    {
        Vector3 direction = (RopeTomahawk.Instance.T2.position - transform.position).normalized;

        float timeMultiplyer = Mathf.Max((Mathf.Log(4, grappleTimer + 1.4f)) / 4f, 0.25f);
        // Debug.Log(timeMultiplyer);
        //body.velocity = body.velocity + direction * (grappleForce * timeMultiplyer) * Time.deltaTime + Vector3.up * upPushInGrapple * Time.deltaTime;
        Vector3 move = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0, input.Player.Move.ReadValue<Vector2>().y);
        if (move != Vector3.zero)
        {
            move = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0) * new Vector3(move.x * moveSpeedInGrapple * Time.deltaTime, 0, move.z * moveSpeedInGrapple * Time.deltaTime);
            body.velocity += move;


        }
        /*if ((body.velocity + direction * (grappleForce * timeMultiplyer) * Time.deltaTime + Vector3.up * upPushInGrapple * Time.deltaTime).magnitude > maxSpeedInGrapple)
        {
            body.velocity = (body.velocity + direction * (grappleForce * timeMultiplyer) * Time.deltaTime).normalized * maxSpeedInGrapple + Vector3.up * 21f * Time.deltaTime;
        }
        else
        {*/

        body.velocity = body.velocity + direction * (grappleForce * timeMultiplyer) * Time.deltaTime + Vector3.up * upPushInGrapple * Time.deltaTime;

        body.velocity -= body.velocity * stoppingDragGrapple * Time.deltaTime;
        //}
    }

    public void Move()
    {

        Vector3 move = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0, input.Player.Move.ReadValue<Vector2>().y);
        if (move != Vector3.zero)
        {
            // orijentise move vektor u pravcu ge gleda igrac
            float velocityChange = wasGrappling ? moveSpeedInGrappleAir : moveSpeed;
            move = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0) * new Vector3(move.x * velocityChange * Time.deltaTime, 0, move.z * velocityChange * Time.deltaTime);
            bool adjustedSpeed = false;
            if (!wasGrappling)
            {
                //gleda koliki je ugao zemlje na kojoj stojis
                Hit hit = ReturnClosestHitSphere(transform.position + Vector3.up * groundCheckoffsetForRaycast, groundCheckRadious, ~excludePlayer);
                if (hit.hit && grounded)
                {
                    float dot = Vector3.Dot(move, hit.normal);
                    float angle = (Vector3.Angle(hit.normal, Vector3.up));

                    if (dot > 0f)
                    {

                        //add velocitty in direction of slope normal
                        move = (move - hit.normal * dot);
                    }
                    else
                    {
                        //if slope too steep, dont add velocity
                        move *= 1 - ((Mathf.Max(Mathf.Min(angle, maxWalkAngle), normalWalkAngle) - normalWalkAngle) / (maxWalkAngle - normalWalkAngle));
                    }
                }
                float magnitude = new Vector3(body.velocity.x, 0, body.velocity.z).magnitude;
                Vector3 tempVelocity = new Vector3(body.velocity.x + move.x, 0, body.velocity.z + move.z);
                Vector3 tmpMove = new Vector3(move.x, 0, move.z);

                float dampen = (Mathf.Max(Mathf.Min(tempVelocity.magnitude, maxSpeed + maxSpeedDampenArea), maxSpeed) - maxSpeed) / maxSpeedDampenArea;

                Vector3 velocityTMP = body.velocity;
                velocityTMP -= velocityTMP * stoppingDrag * Time.deltaTime;
                body.velocity = new Vector3(velocityTMP.x, body.velocity.y, velocityTMP.z);

                if (grounded)
                {
                    if (tempVelocity.magnitude > maxSpeed)
                    {
                        if (magnitude > maxSpeed)
                        {
                            //tempVelocity -= tempVelocity * stoppingDrag * Time.deltaTime;
                            if (tempVelocity.magnitude < maxSpeed)
                                tempVelocity = tempVelocity.normalized * maxSpeed;
                            if (tempVelocity.magnitude > magnitude)
                            {
                                body.velocity = (tempVelocity).normalized * magnitude + new Vector3(0, body.velocity.y, 0);

                            }
                            else
                            {
                                body.velocity = tempVelocity + new Vector3(0, body.velocity.y, 0);
                            }

                        }
                        else
                        {
                            body.velocity = tempVelocity.normalized * maxSpeed + new Vector3(0, body.velocity.y, 0);
                        }
                        adjustedSpeed = true;
                    }
                }
                else
                {
                    if (initiatedJumpByPlayer)
                    {
                        if (tempVelocity.magnitude > maxSpeedInJump)
                        {
                            if (magnitude > maxSpeedInJump)
                            {
                                //tempVelocity -= tempVelocity * stoppingDrag * Time.deltaTime;
                                if (tempVelocity.magnitude < maxSpeedInJump)
                                    tempVelocity = tempVelocity.normalized * maxSpeedInJump;
                                if (tempVelocity.magnitude > magnitude)
                                {
                                    body.velocity = (tempVelocity).normalized * magnitude + new Vector3(0, body.velocity.y, 0);
                                }
                                else
                                {
                                    body.velocity = tempVelocity + new Vector3(0, body.velocity.y, 0);
                                }

                            }
                            else
                            {
                                body.velocity = tempVelocity.normalized * maxSpeedInJump + new Vector3(0, body.velocity.y, 0);
                            }
                            adjustedSpeed = true;
                        }

                    }
                    else
                    {
                        if (tempVelocity.magnitude > maxSpeed)
                        {
                            if (magnitude > maxSpeed)
                            {
                                //tempVelocity -= tempVelocity * stoppingDrag * Time.deltaTime;
                                if (tempVelocity.magnitude < maxSpeed)
                                    tempVelocity = tempVelocity.normalized * maxSpeed;
                                if (tempVelocity.magnitude > magnitude)
                                {
                                    body.velocity = (tempVelocity).normalized * magnitude + new Vector3(0, body.velocity.y, 0);
                                }
                                else
                                {
                                    body.velocity = tempVelocity + new Vector3(0, body.velocity.y, 0);
                                }

                            }
                            else
                            {
                                body.velocity = tempVelocity.normalized * maxSpeed + new Vector3(0, body.velocity.y, 0);
                            }
                            adjustedSpeed = true;
                        }
                    }
                }
            }

            if (!adjustedSpeed)
                body.velocity += move;



            if (wasGrappling)
            {// ako si bio lijanaMan/tarzn do malo pre onda ima sporije padas
                body.velocity += Vector3.up * upPushInGrapple * Time.deltaTime;
            }
        }
        else
        {
            if (!wasGrappling)
            {
                Vector3 velocityTMP = body.velocity;
                velocityTMP -= velocityTMP * stoppingDrag * Time.deltaTime;
                body.velocity = new Vector3(velocityTMP.x, body.velocity.y, velocityTMP.z);
                //body.velocity = (new Vector3(body.velocity.x * stoppingDrag, body.velocity.y, body.velocity.z * stoppingDrag));
            }
            else
            {
                Vector3 velocityTMP = body.velocity;
                velocityTMP -= velocityTMP * stoppingDragGrapple * Time.deltaTime;
                body.velocity = new Vector3(velocityTMP.x, body.velocity.y, velocityTMP.z);
                //body.velocity -= body.velocity * stoppingDragGrapple * Time.deltaTime;
            }
        }
    }

    public void Jump()
    {
        if (jump)
        {
            if (grounded)
            {
                Hit hit = ReturnClosestHitSphere(transform.position + Vector3.up * groundCheckoffsetForRaycast, groundCheckRadious, ~excludePlayer);
                float jumpMultiplyer = 1;// ako je strmo previse, nema skaces bre
                if (hit.hit)
                {
                    float angle = (Vector3.Angle(hit.normal, Vector3.up));
                    //Debug.Log(angle);
                    jumpMultiplyer = 1 - ((Mathf.Max(Mathf.Min(angle, maxWalkAngle), normalWalkAngle) - normalWalkAngle) / (maxWalkAngle - normalWalkAngle));
                    //Debug.Log(jumpMultiplyer);
                    if (jumpMultiplyer > 0.01f)
                    {
                        if (velocity.y > 0)
                        {
                            body.velocity = new Vector3(body.velocity.x, velocity.y + jumpForce * jumpMultiplyer, body.velocity.z);
                        }
                        else
                        {
                            body.velocity = new Vector3(body.velocity.x, jumpForce * jumpMultiplyer, body.velocity.z);

                        }


                        //Debug.Log("mali skok");
                        initiatedJumpByPlayer = true;
                        grounded = false;

                        transform.position += hit.normal * 0.14f;// nemam pojma zasto, al ako ovo maknes sve se pokvari....
                    }
                }

            }
            else
            {
                if (velocity.y > 0)
                {

                }
                else
                {

                }
            }
        }
        else
        {
            if (grounded)
            {
                if (initiatedJumpByPlayer)
                {
                    initiatedJumpByPlayer = false;
                }
            }
            else
            {

            }
        }

    }

    public void Look()
    {
        Vector2 look = input.Player.Look.ReadValue<Vector2>();
        look *= Time.deltaTime * mouseSensetivity;

        float camRotationX = PlayerCamera.transform.rotation.eulerAngles.x >= 270 ? PlayerCamera.transform.rotation.eulerAngles.x - 360f : PlayerCamera.transform.rotation.eulerAngles.x;

        camRotationX -= look.y;
        camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

        PlayerCamera.gameObject.transform.eulerAngles = new Vector3(
            camRotationX,
            PlayerCamera.gameObject.transform.eulerAngles.y,
            PlayerCamera.gameObject.transform.eulerAngles.z);


        transform.Rotate(new Vector3(0, look.x, 0));
    }


    /* public Vector3 moveV = Vector3.zero;
     public Vector3 moveM = Vector3.zero;
     [SerializeField][Range(0.5F, 2)] private float arrowLength = 3.0F;
     private void OnDrawGizmos()
     {
         if (!Application.isPlaying) return;

         var position = transform.position;
         var velocity = moveV;

         if (velocity.magnitude < 0.1f) return;

         Handles.color = Color.red;
         Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(velocity), arrowLength, EventType.Repaint);


         if (!Application.isPlaying) return;

         var position2 = transform.position;
         var velocity2 = moveM;

         if (velocity.magnitude < 0.1f) return;

         Handles.color = Color.blue;
         Handles.ArrowHandleCap(0, position2, Quaternion.LookRotation(velocity2), arrowLength * 2, EventType.Repaint);
     }
 */
    /*
        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + new Vector3(0, groundCheckoffset, 0), groundCheckRadious);
        }*/
}
