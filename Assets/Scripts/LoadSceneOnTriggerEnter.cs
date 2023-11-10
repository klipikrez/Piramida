using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    public SceneReference scene;

    private void OnTriggerEnter(Collider other)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RuntimeSceneManager.Instance.Load(scene);
    }
}
