using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [System.NonSerialized]
    public float time = 52f;
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        if (timer > time)
        {
            Destroy(gameObject);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
