using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicOnTriggerEnter : MonoBehaviour
{
    public string name = "";
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.SetMainMusic(name);
    }
}
