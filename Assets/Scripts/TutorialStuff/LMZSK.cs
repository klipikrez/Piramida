using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMZSK : MonoBehaviour
{
    public static bool isSeenByPlayer = true;
    public static LMZSK Instance;
    private void Awake()
    {
        Instance = this;
    }
    void OnBecameInvisible()
    {
        isSeenByPlayer = false;
    }

    void OnBecameVisible()
    {
        isSeenByPlayer = true;
    }
    private void Update()
    {
        //   Debug.Log(isSeenByPlayer);
    }
}
