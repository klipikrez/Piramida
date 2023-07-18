using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDefaultSrcipts : MonoBehaviour
{
    public List<Object> ScriptsBruh;

    void Awake()
    {
        foreach (Object script in ScriptsBruh)
        {
            Instantiate(script);
        }
    }


}
