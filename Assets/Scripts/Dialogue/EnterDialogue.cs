using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class EnterDialogue : MonoBehaviour
{
    public string startNode = "";
    Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void Update()
    {
        if (DialogueManager.Instance.inDialogue)
        {
            if (rend.enabled)
            {
                rend.enabled = false;
            }
        }
        else
        {
            if (!rend.enabled)
            {
                rend.enabled = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        DialogueManager.Instance.StartDialogue(startNode);
        gameObject.SetActive(false);
    }
}
