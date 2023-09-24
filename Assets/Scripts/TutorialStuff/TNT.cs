using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : BaseEnemy
{
    public GameObject[] destroy;
    public override void Damage(float damage)
    {
        AudioManager.Instance.PlayAudioClip("OdeMuOko");
        foreach (GameObject obj in destroy)
        {

            obj.SetActive(false);

        }
    }
}
