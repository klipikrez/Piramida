using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class LMZSKPositionManager : MonoBehaviour
{
    public static int positionIndex = 0;
    int currentPositionIndex = 0;
    public Transform[] positions;
    GameObject camera;
    public LayerMask mask;

    public static LMZSKPositionManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (camera == null)
        {
            camera = Camera.main.gameObject;
        }
    }
    public static void NextPosition()
    {
        positionIndex++;
    }

    private void Update()
    {

        if (positionIndex == currentPositionIndex)
        {
            return;
        }
        if (DialogueManager.Instance.inDialogue)
        {
            return;
        }
        if (LMZSK.isSeenByPlayer)
        {
            RaycastHit ray;
            if (Physics.Raycast(camera.transform.position, (LMZSK.Instance.gameObject.GetComponent<Renderer>().bounds.center - camera.transform.position).normalized, out ray, ~mask))
            {
                if (ray.collider.gameObject == LMZSK.Instance.gameObject)
                {
                    return;
                }
                else
                {
                    Debug.Log(ray.collider.gameObject.name);
                }
            }
            else
            {
                return;
            }
        }



        ChangePosition();

    }

    public void ChangePosition()
    {

        if (positions.Length > positionIndex)
        {
            LMZSK.Instance.transform.position = positions[positionIndex].position;
            LMZSK.Instance.transform.rotation = positions[positionIndex].rotation;
        }
        else
            Debug.Log("Nema");
        currentPositionIndex = positionIndex;
    }
}
