using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
    //[System.NonSerialized]
    public bool grounded = true;
    //  [System.NonSerialized]
    public bool jump = false;
    [System.NonSerialized]
    public bool initiatedJumpByPlayer = false;
    public float jumpForce = 1f;
    public float jumpPush = 20f;
    public float jumpSpeedBoostMultiply = 2f;
    public Vector3 velocity = Vector3.zero;
    public float maxSpeed = 2;
    public float maxSpeedInJump = 2;
    private float camRotationX = 0;
    public float stoppingDrag = 0.8f;
    public float normalWalkAngle = 45f;
    public float maxWalkAngle = 60f;
    public PhysicMaterial footFo;
    public LayerMask excludePlayer;

    // Start is called before the first frame update
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
    }

    private void StopMove(InputAction.CallbackContext context)
    {

        if (grounded && body.velocity.y > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
            {
                // move = new Vector3(hit.normal.x * move.x, hit.normal.y * move.y, hit.normal.z * move.z);


                body.velocity = body.velocity * ((1 - (Vector3.Angle(hit.normal, Vector3.down) - 90) / 90))  /*new Vector3(0, 0, 0)*/;
            }


        }

    }

    private void JumpStart(InputAction.CallbackContext context)
    {

        //        Debug.Log("jump");
        jump = true;
        if (grounded)
        {

            body.velocity = new Vector3(body.velocity.x * jumpSpeedBoostMultiply, body.velocity.y, body.velocity.z * jumpSpeedBoostMultiply);
        }
    }

    private void JumpEnd(InputAction.CallbackContext context)
    {
        //Debug.Log("ENDjump");
        jump = false;
        if (body.velocity.y > 0)
        {
            body.velocity = new Vector3(body.velocity.x, body.velocity.y / 1.8f, body.velocity.z);
        }
    }


    //int frame = 0;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(frame++ + ":  velovicty:   " + velocity.y);
        Vector3 move = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0, input.Player.Move.ReadValue<Vector2>().y);
        Vector2 look = input.Player.Look.ReadValue<Vector2>();


        //grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), Vector3.down, 0.1f);
        grounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.3f - 0.02f, 0), 0.3f, ~excludePlayer);


        if (jump)
        {
            if (grounded)
            {
                RaycastHit hit;
                float jumpMultiplyer = 1;// ako je strmo previse, nema skaces bre
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
                {
                    float angle = (Vector3.Angle(hit.normal, Vector3.up));

                    jumpMultiplyer = 1 - ((Mathf.Max(Mathf.Min(angle, maxWalkAngle), normalWalkAngle) - normalWalkAngle) / (maxWalkAngle - normalWalkAngle));
                }
                if (velocity.y > 0)
                {
                    //Debug.Log("gor: " + transform.position);
                    body.velocity = new Vector3(body.velocity.x, velocity.y + jumpForce * jumpMultiplyer, body.velocity.z);
                }
                else
                {
                    //Debug.Log("dol: " + transform.position);
                    body.velocity = new Vector3(body.velocity.x, jumpForce * jumpMultiplyer, body.velocity.z);

                }

                initiatedJumpByPlayer = true;
                grounded = false;
                //Debug.Log("VELOCITY:     " + velocity.y);
                transform.position += new Vector3(0, 0.14f, 0);// nemam pojma zasto, al ako ovo maknes sve se pokvari....
            }
            else
            {
                if (velocity.y > 0)
                {
                    /// body.AddForce(0, jumpPush, 0, ForceMode.Force);
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
                //body.add(new Vector3(0, strickToTheGroudForce, 0), ForceMode.Acceleration);
                if (initiatedJumpByPlayer)
                {
                    initiatedJumpByPlayer = false;
                }
                //body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
            }
            else
            {
                //body.velocity = new Vector3(body.velocity.x, body.velocity.x / 1.5f, body.velocity.z);
                //body.AddForce(new Vector3(0, -1, 0), ForceMode.VelocityChange);
            }
            //verticalVelocity += gravity * Time.deltaTime;

        }
        /*

                if ((initiatedJumpByPlayer && grounded) && !jump)
                {
                    initiatedJumpByPlayer = false;
                }*/


        //(new Vector3(body.velocity.x * body.drag, body.velocity.y, body.velocity.z * body.drag))
        if (move != Vector3.zero)
        {
            move = Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y, 0) * new Vector3(move.x * moveSpeed * Time.deltaTime, 0, move.z * moveSpeed * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
            {
                // move = new Vector3(hit.normal.x * move.x, hit.normal.y * move.y, hit.normal.z * move.z);


                float dot = Vector3.Dot(move, hit.normal);
                //Debug.Log(Vector3.Angle(hit.normal, Vector3.up));
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
            //moveV = move;
            body.velocity += move;
            //moveV = move;
            Vector3 tempVelocity = new Vector3(body.velocity.x, 0, body.velocity.z);

            if (grounded)
            {

                if (tempVelocity.magnitude > maxSpeed)
                {

                    body.velocity = (tempVelocity.normalized * maxSpeed) + new Vector3(0, body.velocity.y, 0);
                }
            }
            else
            {
                if (initiatedJumpByPlayer)
                {
                    if (tempVelocity.magnitude > maxSpeedInJump)
                    {

                        body.velocity = (tempVelocity.normalized * maxSpeedInJump) + new Vector3(0, body.velocity.y, 0);
                    }
                }
                else
                {
                    if (tempVelocity.magnitude > maxSpeed)
                    {

                        body.velocity = (tempVelocity.normalized * maxSpeed) + new Vector3(0, body.velocity.y, 0);
                    }
                }
            }
        }
        else
        {
            body.velocity = (new Vector3(body.velocity.x * stoppingDrag, body.velocity.y, body.velocity.z * stoppingDrag));
        }

        look *= Time.deltaTime * mouseSensetivity;
        //body.velocity = (new Vector3(velocity.x, /*verticalVelocity*/body.velocity.y, velocity.z));

        camRotationX -= look.y;
        camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

        PlayerCamera.gameObject.transform.eulerAngles = new Vector3(
            camRotationX,
            PlayerCamera.gameObject.transform.eulerAngles.y,
            PlayerCamera.gameObject.transform.eulerAngles.z);


        transform.Rotate(new Vector3(0, look.x, 0));
        //PlayerCamera.gameObject.transform.Rotate(new Vector3(-look.y, 0, 0));
        velocity = body.velocity;
        //moveM = body.velocity;
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
    /* void OnCollisionStay(Collision collisionInfo)
     {







     }

     void OnCollisionEnter(Collision col)
     {
         Debug.Log("==============");
         // Debug-draw all contact points and normals
         foreach (ContactPoint contact in col.contacts)
         {

             Debug.Log(contact.thisCollider.material);
             Debug.Log(contact.otherCollider.material);

             if (contact.thisCollider.material = footFo)
             {
                 grounded = true;
             }
         }
         Debug.Log("==============");
     }

     void OnCollisionExit(Collision col)
     {
         grounded = false;
     }*/


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.3f - 0.02f, 0), 0.3f);
    }
}
