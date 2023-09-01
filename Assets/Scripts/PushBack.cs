using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBack : MonoBehaviour
{
    public float force = 52f;
    public float upForce = 21f;
    void OnTriggerEnter(Collider other)
    {
        //        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {

            other.attachedRigidbody.velocity =
            (new Vector3(other.transform.position.x, 0, other.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized
            * force + transform.up * upForce;
        }
    }
}
