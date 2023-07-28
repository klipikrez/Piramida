using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnStayTrigger : MonoBehaviour
{
    public float damage = 52f;
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {

            other.attachedRigidbody.gameObject.GetComponent<PlayerStats>().ContinuousDamage(damage);

        }
    }
}