using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    public SceneReference scene;

    private void OnTriggerEnter(Collider other)
    {
        RuntimeSceneManager.Instance.Load(scene);
    }
}
