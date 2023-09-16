using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDefaultSrcipts : MonoBehaviour
{
    public List<Object> ScriptsBruh;
    public GameObject runtimeSceneManager;

    void Awake()
    {
        foreach (Object script in ScriptsBruh)
        {
            Instantiate(script);
        }

        if (Object.FindObjectOfType<RuntimeSceneManager>() == null)
        {
            Instantiate(runtimeSceneManager);
        }
    }


}
