using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOnGround : MonoBehaviour
{
    public float time = 5f;
    public float timer = 0;
    public float size = 4;
    public static bool doneDamage = false;//ovo ti da moz samo jedan collider u jednom frejmu da te osteti.
    public float damage = 20f;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.localScale = Vector3.one * size * (1 - (timer / time));
        if (timer > time)
        {
            Destroy(gameObject);
        }
        else
        {

            if (doneDamage)
            {
                doneDamage = false;
                //Debug.Log("===============");
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!doneDamage && other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            //Debug.Log("DAMAGE");
            other.gameObject.GetComponentInParent<PlayerStats>().ContinuousDamage(damage);
            if (!doneDamage)
                doneDamage = true;
        }
    }
}
