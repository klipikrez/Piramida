using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnTriggerEnter : BaseEnemy
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

    public override void Damage(float damage)
    {


        foreach (GameObject obj in ActivateOnEnter)
        {
            obj.SetActive(true);
        }
        Destroy(gameObject);

    }
}