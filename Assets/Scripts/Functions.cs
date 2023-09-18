using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hit
{
    public Collider collider;
    public Vector3 point = Vector3.zero;
    public Vector3 normal = Vector3.zero;
    public float distance = float.MaxValue;
    public bool hit = false;
}

public class Box
{
    public Vector3 center;
    public Vector3 bounds;
    public quaternion rotation;
}

public class Settings
{
    public bool fullScreen = true;
    public int fps = 60;
    public float[] volumes = { 1, 1, 1, 1 };
    public bool vsync = false;
    public string language = "bs";
    /*public float volume0 = 1;
    public float volume1 = 1;
    public float volume2 = 1;
    public float volume3 = 1;*/

}

public static class Functions
{
    private static SphereCollider sphere;
    static Functions()
    {

        SphereCollider sphereTMP = new GameObject("SphereColliderDistanceCalculator(See in Functions.cs)").AddComponent<SphereCollider>();
        sphereTMP.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        sphereTMP.radius = 2f;
        sphereTMP.center = Vector3.zero;
        sphereTMP.isTrigger = true;

        sphere = sphereTMP;
        Object.DontDestroyOnLoad(sphere);
    }
    public static float DeltaTimeLerp(float value)
    {
        //ovo ti radi stvar, tako da lerp zavisi od vremena, a ne da vraca samo fiksnu vrednost svakog frejma
        return 1 - Mathf.Pow(1 - value, Time.deltaTime * 60);
    }

    public static Hit ReturnClosestHitSphere(Vector3 origin, float CheckRadious = 0.4f, int mask = 0)
    {


        Hit hit = new Hit();

        Collider[] colliders = Physics.OverlapSphere(origin, CheckRadious, mask);
        foreach (Collider col in colliders)
        {
            //if (!col.isTrigger)
            //{

            if (CheckSphereExtra(col, origin, CheckRadious, out Vector3 point, out Vector3 normal))
            {
                float distance = Vector3.Distance(point, origin);

                if (distance < hit.distance)
                {
                    hit.distance = distance;

                    hit.point = point;
                    hit.normal = normal;
                    hit.collider = col;
                }

                hit.hit = true;
                /*skaj*/
            }
            // }
        }
        return hit;
    }

    public static Hit ReturnClosestHitBoxExclude(Vector3 origin, List<Collider> collidersToExclude, out List<Collider> checkedColliders, Quaternion rotation, Vector3 CheckBounds, int mask = 0)
    {

        checkedColliders = new List<Collider>();
        Hit hit = new Hit();

        Collider[] colliders = Physics.OverlapBox(origin, CheckBounds, rotation, mask);
        foreach (Collider col in colliders)
        {
            //if (!col.isTrigger)
            //{
            if (!collidersToExclude.Contains(col))
            {
                if (CheckSphereExtra(col, origin, CheckBounds.magnitude, out Vector3 point, out Vector3 normal))
                {
                    float distance = Vector3.Distance(point, origin);

                    if (distance < hit.distance)
                    {
                        hit.distance = distance;

                        hit.point = point;
                        hit.normal = normal;
                        checkedColliders.Add(col);
                        hit.collider = col;
                    }

                    hit.hit = true;
                    /*skaj*/
                }

            }
            //}
        }
        return hit;
    }

    public static Box CalculateBoxBounds(Vector3 currentPosition, Vector3 CalculatedPosition, Vector3 boxOriginalBounds)
    {
        Box box = new Box();

        float DistanceAboutToBeTraveled = Vector3.Distance(currentPosition, CalculatedPosition);
        box.rotation = Quaternion.LookRotation(CalculatedPosition - currentPosition);
        box.bounds = new Vector3(boxOriginalBounds.x, boxOriginalBounds.y, DistanceAboutToBeTraveled);
        box.center = currentPosition + new Vector3(0, 0, DistanceAboutToBeTraveled / 2);

        return box;
    }
    /*public static RaycastHit ReturnClosestHitOLD(Vector3 origin, SphereCollider sphere, float groundCheckRadious = 0.4f, int mask = 0)
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
                float distance = 52.0025052f;

                //if (col.GetType() == typeof(MeshCollider) && !((MeshCollider)col).convex)
                //{
                Vector3 point;
                Vector3 TRASH;
                CheckSphereExtra(col, origin, out point, out TRASH);
                distance = Vector3.Distance(point, origin);
                //}
                //else
                //{
                //    distance = Vector3.Distance(col.ClosestPoint(origin), origin);
                //}
                if (distance < closestPointDistance)
                {
                    closestPointDistance = distance;

                    closestColliderPoint = point;//col.ClosestPoint(origin);
                }
                completed = true;
            }
        }
        //        Debug.Log(closestColliderPoint);
        if (completed)
        {

            if (Physics.Raycast(origin, (closestColliderPoint - (origin)).normalized, out hit, 10f))
            {
                return hit;
            }

        }
        //        Debug.Log("nis nes pogodeo");
        hit.distance = float.MaxValue;
        return hit; //Vector3.negativeInfinity;
    }*/

    public static bool CheckSphereExtra(Collider target_collider, Vector3 position, float CheckRadious, out Vector3 closest_point, out Vector3 surface_normal)
    {
        closest_point = Vector3.zero;
        surface_normal = Vector3.zero;
        sphere.transform.position = position;
        sphere.radius = CheckRadious + 1f;

        if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere, position, Quaternion.identity, out surface_normal, out float surface_penetration_depth))
        {
            closest_point = position + (surface_normal * (sphere.radius - surface_penetration_depth));

            surface_normal = -surface_normal;

            return true;
        }

        return false;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }




}
