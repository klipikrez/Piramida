using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity.Example;

public class CheckIfBulletInside : MonoBehaviour
{
    public static bool bullet = false;
    public BoxCollider coll;
    public static CheckIfBulletInside Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            bullet = true;
        }
    }

    private void Update()
    {
        if (RopeTomahawk.Instance.T2 != null && IsInsideBoxCollider(coll, RopeTomahawk.Instance.T2.position))
        {
            bullet = true;
        }
    }

    public static bool IsInsideBoxCollider(BoxCollider aCol, Vector3 aPoint)
    {
        var b = new Bounds(aCol.center, aCol.size * 0.5f);
        var p = aCol.transform.InverseTransformPoint(aPoint);
        return b.Contains(p);
    }
    /*
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Bullet")
            {
                bullet = false;
            }
        }*/
}
