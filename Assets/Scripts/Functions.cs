using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Functions
{
    public static float DeltaTimeLerp(float value)
    {
        //ovo ti radi stvar, tako da lerp zavisi od vremena, a ne da vraca samo fiksnu vrednost svakog frejma
        return 1 - Mathf.Pow(1 - value, Time.deltaTime * 60);
    }

    public static RaycastHit ReturnClosestHit(Vector3 origin, float groundCheckRadious = 0.4f)
    {


        float closestPointDistance = float.MaxValue;
        Vector3 closestColliderPoint = Vector3.zero;

        RaycastHit hit = new RaycastHit();

        bool completed = false;
        Collider[] colliders = Physics.OverlapSphere(origin, groundCheckRadious, ~LayerMask.GetMask("Hitbox", "Player", "Attack", "Bullet", "Ford", "Mazda"));
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                if (col.GetType() == typeof(MeshCollider) && !((MeshCollider)col).convex)
                {
                    //ako je sjean, ne diraj ga za sad
                    Debug.LogError("nevalja ti collider eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
                }
                else
                {
                    float distance = Vector3.Distance(col.ClosestPoint(origin), origin);
                    if (distance < closestPointDistance)
                    {
                        closestPointDistance = distance;

                        closestColliderPoint = col.ClosestPoint(origin);
                    }
                    completed = true;
                }
            }
        }
        if (completed)
        {

            if (Physics.Raycast(origin, (closestColliderPoint - (origin)).normalized, out hit, 10f))
            {
                return hit;
                /*
                                //hit.normal = Quaternion.AngleAxis(45, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized
                                //                Debug.Log(Vector3.Angle(hit.normal, Vector3.down));
                                if (Vector3.Angle(hit.normal, Vector3.down) > 90f && Vector3.Angle(hit.normal, Vector3.down) < 270f)
                                {
                                    return hit;
                                }
                                else
                                {
                                    if (Vector3.Angle(hit.normal, Vector3.down) < 90f)
                                    {
                                        //Debug.Log(Quaternion.AngleAxis(90, transform.right) * (hit.normal) + "   +/////////////////////////////////////////////////");
                                        //return Quaternion.AngleAxis(90, transform.right) * (hit.normal);
                                    }
                                    else
                                    {
                                        if (Vector3.Angle(hit.normal, Vector3.down) > 270f)
                                        {
                                            Vector3 temp = hit.normal - Vector3.up * hit.normal.y;
                                            Debug.Log(temp.normalized + "   /////////////////////////////////////////////////");
                                            return temp.normalized;
                                        }
                                    }
                                }*/
            }

        }
        //        Debug.Log("nis nes pogodeo");
        hit.distance = float.MaxValue;
        return hit; //Vector3.negativeInfinity;
    }





    public static RaycastHit ReturnClosestHit(Vector3 origin, float groundCheckRadious = 0.4f, int mask = 0)
    {


        float closestPointDistance = float.MaxValue;
        Vector3 closestColliderPoint = Vector3.zero;

        RaycastHit hit = new RaycastHit();

        bool completed = false;
        Collider[] colliders = Physics.OverlapSphere(origin, groundCheckRadious, mask);
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                if (col.GetType() == typeof(MeshCollider) && !((MeshCollider)col).convex)
                {
                    //ako je sjean, ne diraj ga za sad
                    Debug.LogError("nevalja ti collider eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
                }
                else
                {
                    float distance = Vector3.Distance(col.ClosestPoint(origin), origin);
                    if (distance < closestPointDistance)
                    {
                        closestPointDistance = distance;

                        closestColliderPoint = col.ClosestPoint(origin);
                    }
                    completed = true;
                }
            }
        }
        if (completed)
        {

            if (Physics.Raycast(origin, (closestColliderPoint - (origin)).normalized, out hit, 10f))
            {
                return hit;
                /*
                                //hit.normal = Quaternion.AngleAxis(45, bullet.transform.right) * (bullet.employer.transform.position - bullet.transform.position).normalized
                                //                Debug.Log(Vector3.Angle(hit.normal, Vector3.down));
                                if (Vector3.Angle(hit.normal, Vector3.down) > 90f && Vector3.Angle(hit.normal, Vector3.down) < 270f)
                                {
                                    return hit;
                                }
                                else
                                {
                                    if (Vector3.Angle(hit.normal, Vector3.down) < 90f)
                                    {
                                        //Debug.Log(Quaternion.AngleAxis(90, transform.right) * (hit.normal) + "   +/////////////////////////////////////////////////");
                                        //return Quaternion.AngleAxis(90, transform.right) * (hit.normal);
                                    }
                                    else
                                    {
                                        if (Vector3.Angle(hit.normal, Vector3.down) > 270f)
                                        {
                                            Vector3 temp = hit.normal - Vector3.up * hit.normal.y;
                                            Debug.Log(temp.normalized + "   /////////////////////////////////////////////////");
                                            return temp.normalized;
                                        }
                                    }
                                }*/
            }

        }
        //        Debug.Log("nis nes pogodeo");
        hit.distance = float.MaxValue;
        return hit; //Vector3.negativeInfinity;
    }


}
