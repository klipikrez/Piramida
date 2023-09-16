using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    public SceneReference scene;
    public void Load()
    {
        RuntimeSceneManager.Instance.Load(scene);
    }
}
