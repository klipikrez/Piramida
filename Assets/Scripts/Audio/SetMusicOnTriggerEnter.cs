using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicOnTriggerEnter : MonoBehaviour
{
    public string name = "";
    public MusicSet musicset;
    private void OnTriggerEnter(Collider other)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.Instance.SetMainMusicSet(musicset);
    }
}
