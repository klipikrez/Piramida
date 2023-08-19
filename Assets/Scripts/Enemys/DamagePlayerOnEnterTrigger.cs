using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnEnterTrigger : MonoBehaviour
{
    public float damage = 52f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            other.attachedRigidbody.gameObject.GetComponent<PlayerStats>().Damage(damage, gameObject);
        }
    }
}
