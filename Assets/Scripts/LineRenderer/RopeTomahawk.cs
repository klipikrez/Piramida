using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

public class RopeTomahawk : MonoBehaviour
{
    public Transform T1;
    public Vector3 T1Offset;
    public Transform T2;
    [System.NonSerialized]
    public LineRenderer lineRenderer;
    public static RopeTomahawk Instance;
    public float spinSpeed = 52f;
    public bool hit = false;
    public bool reloading = false;
    public float hitTime = 0;
    public List<Vector3> path = new List<Vector3>();
    public List<float> pathDistances = new List<float>();
    public float distance;
    public float pathInterval = 0.2f;
    public float airTime = 0;
    public AnimationCurve kurva;
    public float multiplyShakeWhenhit = 6f;
    float combinedLenth = 0;
    bool reachedMaxRopeLenth = false;
    public float maxRopeLenth = 52f;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (T1 != null && T2 != null)
        {

            if (!reloading)
            {
                if (hit)
                {
                    HitWithPath();
                }
                else
                {
                    SpinWithPath();
                    airTime += Time.deltaTime;
                }
            }
            else
            {
                HitWithPath();
                //PullPlayerTowardsHitPoint();
            }
        }
    }

    public void SpinWithPath()
    {


        UpdateLinePointCount();

        SetLineRendererPoints();

        OffsetLineRendererPoints();


    }

    void UpdateLinePointCount()
    {
        if (pathInterval < airTime && !reachedMaxRopeLenth)
        {
            airTime = 0;
            AddPathLink(T2.transform.position);
            if (combinedLenth > maxRopeLenth)
            {
                reachedMaxRopeLenth = true;
            }
        }
    }

    void SetLineRendererPoints()
    {
        lineRenderer.positionCount = 2 + path.Count;

        if (path.Count != 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                //prvo mu lepo setuj pozicije na LineRenderer pa ih onda razdrkaj ko covek
                lineRenderer.SetPosition(i + 1, path[i]);

                //ovde gi grkas
                //ovo ti je za distancu izmedju tacaka, da se medjusobno vucu, ako se odalje previse. to ti je onaj efekat kao da te uze prati || imas ovu kormulu na desmos-u ako ne razumes klipice
                float LerpPointCohesion = (/*(1f / (i + 1f)) **/ (1f / (i + 1f)) / 7f) * (Mathf.Min((Mathf.Max(Vector3.Distance(path[i], i == 0 ? T1.position : path[i - 1]) - pathDistances[i], 0)) / (pathDistances[i] / 2f), 1));

                Vector3 tempPathPoint = Vector3.Lerp(path[i], T1.position, DeltaTimeLerp(LerpPointCohesion));
                //ovo sluzi da ispravlja tacke. da izgleda kao da uze pada na dol, i da na njega deluje gravitacija
                tempPathPoint = Vector3.Lerp(tempPathPoint, PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)), DeltaTimeLerp(0.01f));
                path[i] = tempPathPoint;
            }
        }

        //ovo postavi pozicije prvoj i poslednjoj tacki u LineRender
        lineRenderer.SetPosition(0, T1.position + T1Offset);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
    }

    void OffsetLineRendererPoints()
    {
        //prodjemo jos jednom kroz sve tacke kako bi dodali onaj efekat kao da uze viori i okrece u vazduhu
        //imas funkcije u desmos ako nes ne razumes klipice
        for (int i = 1; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
            Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

            lineRenderer.SetPosition(i,
            path[i - 1] + (offset * 1f
                  * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)));

        }
    }

    public void PullPlayerTowardsHitPoint()
    {

    }

    IEnumerator c_Pull()
    {
        while (reloading)
        {
            // reloadTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //reloadCorutine = null;
    }
    public void HitWithPath()
    {


        airTime = 0;


        lineRenderer.positionCount = 2 + path.Count;


        if (path.Count != 0)
        {
            for (int i = 0; i < path.Count; i++)
            {

                Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
                Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

                //ispravlja sve tacke u pravu liniju
                path[i] = Vector3.Lerp(path[i], PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)), DeltaTimeLerp(Mathf.Min(hitTime * 3, 1)));

                //kako se uze trese kad pogodi nesto
                lineRenderer.SetPosition(i + 1, path[i] + (offset
                                                         * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)//imas u desmos pa glidi tamo
                                                         * Mathf.Cos(hitTime * 55/*interval oscilacije*/)
                                                         * kurva.Evaluate(hitTime)//kurva
                                                         * multiplyShakeWhenhit)
                                                         * Mathf.Clamp01(1.5f - hitTime));//posle 1.5s nema nis da se pomera
            }
        }
        //ovo postavi pozicije prvoj i poslednjoj tacki u LineRender
        lineRenderer.SetPosition(0, T1.position + T1Offset);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
        hitTime += Time.deltaTime;
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
            pathDistances.Add(Vector3.Distance(path[path.Count - 1], T1.transform.position + new Vector3(0, 0.4f, 0)));
        }
        combinedLenth += pathDistances[pathDistances.Count - 1];
    }

    public void RemovePathLink()
    {

    }

    public void ClearPath()
    {
        reachedMaxRopeLenth = false;
        combinedLenth = 0;
        path.Clear();
        pathDistances.Clear();
    }

    public Vector3 PointAtRatio(Vector3 p1, Vector3 p2, float r1, float r2)
    {//ovo, ovo, ovo ti sluzi da rasporedi tacku izmedju de tacke u odredjenor razmeri r1:r2
        return new Vector3(
        (r1 * p2.x + r2 * p1.x) / (r1 + r2),
        (r1 * p2.y + r2 * p1.y) / (r1 + r2),
        (r1 * p2.z + r2 * p1.z) / (r1 + r2));

    }

    public void SetTransformsToFollow(Transform T1, Transform T2)
    {
        this.T1 = T1;
        this.T2 = T2;
    }

    public void ReleaseTransformsToFollow()
    {
        T1 = null;
        T2 = null;
    }
    /*
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
        }*/

}
