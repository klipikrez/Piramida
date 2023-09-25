using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class EnterDialogue : MonoBehaviour
{
    public string startNode = "";
    public int positionIndexBefore = 0;
    public int positionIndexAfter = 1;
    public bool show = true;
    Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void Update()
    {
        if (show)
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
        LMZSKPositionManager.positionIndex = positionIndexBefore;
        LMZSKPositionManager.Instance.ChangePosition();
        DialogueManager.Instance.StartDialogue(startNode);
        LMZSKPositionManager.positionIndex = positionIndexAfter;
        gameObject.SetActive(false);
    }
}
