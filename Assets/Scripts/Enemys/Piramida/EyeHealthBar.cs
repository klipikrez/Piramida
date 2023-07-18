using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHealthBar : MonoBehaviour
{
    MeshRenderer mesh;
    private void Start()
    {
        mesh = gameObject.GetComponent<MeshRenderer>();
    }
    public void SetHealth(bool yer)
    {
        mesh.enabled = yer;
    }
}
