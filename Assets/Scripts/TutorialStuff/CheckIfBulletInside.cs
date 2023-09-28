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

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bullet")
        {
            bullet = true;
        }
    }

    private void Update()
    {
        Debug.Log(RopeTomahawk.Instance.T2 == null ? "null" : RopeTomahawk.Instance.T2 + "  -  " + coll.bounds.Contains(RopeTomahawk.Instance.T2.position));
        if (RopeTomahawk.Instance.T2 != null && coll.bounds.Contains(RopeTomahawk.Instance.T2.position) && PiuPiu.Instance.killed && !bullet)
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
