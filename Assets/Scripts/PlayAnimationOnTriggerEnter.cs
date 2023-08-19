using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnTriggerEnter : MonoBehaviour
{
    bool triggerEnter = false;
    public GameObject[] ActivateOnEnter;
    private void Awake()
    {

        foreach (GameObject obj in ActivateOnEnter)
        {
            obj.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            if (!triggerEnter)
            {
                triggerEnter = true;
                foreach (GameObject obj in ActivateOnEnter)
                {
                    obj.SetActive(true);
                }
            }
    }

}
