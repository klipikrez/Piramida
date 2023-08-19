using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ColliderRedirectEvent : UnityEvent<Collider> { }

public class ColiderRedirector : MonoBehaviour
{
    public ColliderRedirectEvent onTriggerEnter = new ColliderRedirectEvent();


    void OnTriggerEnter(Collider col)
    {
        if (onTriggerEnter != null)
        {
            for (int i = 0; i < onTriggerEnter.GetPersistentEventCount(); i++)
            {
                ((MonoBehaviour)onTriggerEnter.GetPersistentTarget(i)).SendMessage(onTriggerEnter.GetPersistentMethodName(i), col);
            }

        }
    }
}
