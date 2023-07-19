using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTomahawk : MonoBehaviour
{
    public Transform T1;
    public Vector3 T1Offset;
    public Transform T2;
    [System.NonSerialized]
    public LineRenderer lineRenderer;
    public float ropeResolution = 1f;
    // Start is called before the first frame update
    public static RopeTomahawk Instance;
    public float spinSpeed = 52f;
    public bool hit = false;
    public float hitTime = 0;
    public List<Vector3> path = new List<Vector3>();
    public List<float> pathDistances = new List<float>();
    public float pathInterval = 0.2f;
    public float pathT1Influence = 10f; //0 je min brzina priblizavanja ka igracu, zanci ne menja se
    public float airTime = 0;
    public AnimationCurve kurva;
    public float multiplyShakeWhenhit = 6f;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        //InvokeRepeating("UpdateRope", 0f, 1f / 60f);
    }
    // Update is called once per frame
    void Update()
    {

        if (hit)
        {
            //ClearPath();
            HitWithPath();
        }
        else
        {

            SpinWithPath();
            airTime += Time.deltaTime;
        }
    }
    public void Spin()
    {
        if (T1 != null && T2 != null)
        {
            lineRenderer.SetPosition(0, T1.position + T1Offset);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
                Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

                lineRenderer.SetPosition(i,
                PointAtRatio(T1.position + T1Offset, T2.position, i, lineRenderer.positionCount - (i)) + (offset/*new Vector3(
                    Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f),
                     Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f),
                      0)*/
                      * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)));
                // Mathf.Min(Mathf.Max((((lineRenderer.positionCount - 1)/2 - Mathf.Abs(i-(lineRenderer.positionCount - 1)/2))-5),0),1)
            }
        }
    }
    public void SpinWithPath()
    {
        if (T1 != null && T2 != null)
        {

            if (pathInterval < airTime)
            {
                airTime = 0;
                AddPathLink(T2.transform.position);
            }

            lineRenderer.positionCount = 2 + path.Count;


            if (path.Count != 0)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    /*ovo ti je za distancu izmedju tacaka*/
                    float prviLerpTempVerdnost = (/*(1f / (i + 1f)) **/ (1f / (i + 1f)) / 7f) * (Mathf.Min((Mathf.Max(Vector3.Distance(path[i], i == 0 ? T1.position : path[i - 1]) - pathDistances[i], 0)) / (pathDistances[i] / 2f), 1));
                    lineRenderer.SetPosition(i + 1, path[i]);
                    path[i] =
                    Vector3.Lerp(
                         Vector3.Lerp(path[i], T1.position, 1 - Mathf.Pow(1 - (prviLerpTempVerdnost), Time.deltaTime * 60))
                         , PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)),
                          1 - Mathf.Pow(1 - 0.01f, Time.deltaTime * 60))


                    ;
                    //ebug.Log(i + "   ---   " + (Mathf.Min((Mathf.Max(Vector3.Distance(path[i], i == 0 ? T1.position : path[i - 1]) - pathDistances[i], 0)) / (pathDistances[i] / 2f), 1)));

                }
            }
            lineRenderer.SetPosition(0, T1.position + T1Offset);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);

            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
                Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

                lineRenderer.SetPosition(i,
                path[i - 1] + (offset * 1f
                      * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)));

            }
        }
    }
    public void HitWithPath()
    {
        if (T1 != null && T2 != null)
        {

            airTime = 0;


            lineRenderer.positionCount = 2 + path.Count;


            if (path.Count != 0)
            {
                for (int i = 0; i < path.Count; i++)
                {

                    Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
                    Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);


                    path[i] = Vector3.Lerp(path[i], PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)), 1 - Mathf.Pow(1 - Mathf.Min(hitTime * 3, 1), Time.deltaTime * 60));

                    /*ovo ti je za distancu izmedju tacaka*/
                    lineRenderer.SetPosition(i + 1, path[i] + (offset
                                                             * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)
                                                             * Mathf.Cos(hitTime * 55/*interval oscilacije*/)
                                                             * kurva.Evaluate(hitTime)
                                                             * multiplyShakeWhenhit)
                                                             * Mathf.Clamp01(1.5f - hitTime));

                    //ebug.Log(i + "   ---   " + (Mathf.Min((Mathf.Max(Vector3.Distance(path[i], i == 0 ? T1.position : path[i - 1]) - pathDistances[i], 0)) / (pathDistances[i] / 2f), 1)));

                }
            }
            lineRenderer.SetPosition(0, T1.position + T1Offset);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
            hitTime += Time.deltaTime;
        }
    }

    public void Hit()
    {
        if (T1 != null && T2 != null)
        {
            lineRenderer.SetPosition(0, T1.position + T1Offset);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
                Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

                lineRenderer.SetPosition(i,
                PointAtRatio(T1.position + T1Offset, T2.position, i, lineRenderer.positionCount - (i)) + (offset
                      * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)
                      * Mathf.Cos(hitTime * 55/*interval oscilacije*/)
                      * Mathf.Min(Mathf.Log(hitTime + 1, 1.02f/*brzinja opadanja log*/), 1)
                      * Mathf.Max(1f/*brzina opadanja linearna*/- hitTime * 4, 0)
                      ));


            }
            hitTime += Time.deltaTime;
        }
    }

    public void AddPathLink(Vector3 point)
    {
        path.Add(point);
        if (path.Count != 1)
        {
            pathDistances.Add(Vector3.Distance(path[path.Count - 2], path[path.Count - 1]));
        }
        else
        {
            pathDistances.Add(Vector3.Distance(path[path.Count - 1], T1.transform.position + new Vector3(0, 1f, 0)));
            //athDistances.Add(0.2f);
        }
    }
    public void RemovePathLink()
    {

    }
    public void ClearPath()
    {
        path.Clear();
        pathDistances.Clear();
    }
    public Vector3 PointAtRatio(Vector3 p1, Vector3 p2, float r1, float r2)
    {
        //if (r1 != r2)
        //{
        return new Vector3(
        (r1 * p2.x + r2 * p1.x) / (r1 + r2),
        (r1 * p2.y + r2 * p1.y) / (r1 + r2),
        (r1 * p2.z + r2 * p1.z) / (r1 + r2));
        //}
        //else
        //{
        //    return new Vector3(
        //        (p1.x + p2.x) / 2,
        //       (p1.y + p2.y) / 2,
        //        (p1.z + p2.z) / 2
        //88    );
        // }
    }

    void OnDrawGizmosSelected()
    {


        if (T1 != null && T2 != null)
        {



            Gizmos.color = Color.yellow;


            if (path.Count != 0)
            {
                for (int i = 0; i < path.Count; i++)
                {

                    Gizmos.DrawSphere(path[i], 1);

                }
            }

        }

        // Draw a yellow sphere at the transform's position

    }
}
