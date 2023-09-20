using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functions;

public class RopeTomahawk : MonoBehaviour
{
    public Transform T1;
    public PlayerMovement movement;
    public Vector3 T1Offset;
    public Transform T2;
    [System.NonSerialized]
    public LineRenderer lineRenderer;
    public static RopeTomahawk Instance;
    public float spinSpeed = 52f;
    public bool hit = false;
    public bool reloading = false;
    public float hitTime = 0;
    public float bounceTime = 0;
    public List<Vector3> path = new List<Vector3>();
    public List<float> pathDistances = new List<float>();
    public float distance;
    public float pathInterval = 0.2f;
    public float airTime = 0;
    public AnimationCurve kurvaHit;
    public AnimationCurve kurvaBounceBack;
    public float multiplyShakeWhenhit = 6f;
    float combinedLenth = 0;
    public bool reachedMaxRopeLenth = false;
    public float maxRopeLenth = 52f;
    bool returnToSender = false;

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
                    //                    Debug.Log("Max: " + maxRopeLenth + " -- Calculated: " + combinedLenth + " -- Points: " + Vector3.Distance(T1.position, T2.position));

                    if (reachedMaxRopeLenth)
                    {
                        MaxLenthWithPath();
                    }
                    else
                    {
                        SpinWithPath();

                    }
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

    void SpinWithPath()
    {
        UpdatePointCount();

        SetPoints();

        OffsetPoints();
    }

    void MaxLenthWithPath()
    {
        if (!returnToSender)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                path[i] = Vector3.Lerp(path[i], PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)), DeltaTimeLerp(0.1f));
            }
        }

        SetPoints();

        OffsetPoints();

        if (returnToSender)
        {
            BounceBackWithPath();
        }

        if (Vector3.Distance(T1.position, T2.position) > maxRopeLenth)
        {
            AudioManager.Instance.PlayAudioClip("BounceBack", 0.5f);
            returnToSender = true;
            T2.position = (T2.position - T1.position).normalized * (maxRopeLenth - 2f) + T1.position;
            Bullet tomahawkBullet = T2.GetComponent<Bullet>();
            Rigidbody body = T1.GetComponent<Rigidbody>();
            tomahawkBullet.velocity = (T1.position - T2.position).normalized * tomahawkBullet.velocity.magnitude;
            if (AreVectorsSimilar(tomahawkBullet.velocity, body.velocity))
            {
                tomahawkBullet.velocity += body.velocity;
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                path[i] = PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1));
            }
        }



    }

    void UpdatePointCount()
    {
        if (pathInterval < airTime && !reachedMaxRopeLenth)
        {
            airTime = 0;
            AddPathLink(T2.transform.position);
        }

        if (path.Count >= 1)
            if (combinedLenth + Vector3.Distance(T2.position, path[path.Count - 1]) > maxRopeLenth)
            {
                reachedMaxRopeLenth = true;
                pathDistances.Add(maxRopeLenth - combinedLenth);
                combinedLenth = maxRopeLenth;
                //pathDistances.Add(Vector3.Distance(T2.position, path[path.Count - 1]));
                //combinedLenth += Vector3.Distance(T2.position, path[path.Count - 1]);
            }
    }

    void SetPoints()
    {
        lineRenderer.positionCount = 2 + path.Count;

        if (path.Count != 0)
        {

            for (int i = 0; i < 1; i++)
            {
                PullPointsTowardT1();

                if (reachedMaxRopeLenth)
                {
                    PullPointsTowardT2();
                }
            }


            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, path[i]);
            }

        }

        //ovo postavi pozicije prvoj i poslednjoj tacki u LineRender
        lineRenderer.SetPosition(0, T1.position + T1Offset);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);
    }

    void PullPointsTowardT1()
    {
        Vector3 tempPathPoint = Vector3.zero;
        for (int i = 0; i < path.Count; i++)
        {//vuce tacke ka pocetnoj tacki

            if (i == 0)
            {
                tempPathPoint = PullPointTowardsPreviousPoint(T1.position, path[i], pathDistances[i]);
            }
            else
            {
                tempPathPoint = PullPointTowardsPreviousPoint(path[i - 1], path[i], pathDistances[i]);
            }

            //ovo sluzi da ispravlja tacke. da izgleda kao da uze pada na dol, i da na njega deluje gravitacija
            tempPathPoint = Vector3.Lerp(tempPathPoint, PointAtRatio(T1.position + T1Offset, T2.position, i + 1, lineRenderer.positionCount - (i + 1)), DeltaTimeLerp(0.01f));
            path[i] = tempPathPoint;

        }
    }

    void PullPointsTowardT2()
    {
        Vector3 tempPathPoint = Vector3.zero;
        tempPathPoint = PullPointTowardsPreviousPoint(T2.position, path[path.Count - 1], pathDistances[pathDistances.Count - 1]);
        path[path.Count - 1] = tempPathPoint;

        for (int i = path.Count - 1; i >= 1; i--)
        {//vuce tacke ka sekiri


            tempPathPoint = PullPointTowardsPreviousPoint(path[i], path[i - 1], pathDistances[i]);

            path[i - 1] = tempPathPoint;
        }
    }
    Vector3 PullPointTowardsPreviousPoint(Vector3 p1, Vector3 p2, float maxDistance)
    {
        if (Vector3.Distance(p1, p2) > maxDistance)
        {
            p2 = (p2 - p1).normalized * maxDistance + p1;
        }

        return p2;
    }

    void OffsetPoints()
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

    void HitWithPath()
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
                                                         * kurvaHit.Evaluate(hitTime)//kurva
                                                         * multiplyShakeWhenhit)
                                                         * Mathf.Clamp01(1.5f - hitTime));//posle 1.5s nema nis da se pomera
            }
        }
        //ovo postavi pozicije prvoj i poslednjoj tacki u LineRender
        lineRenderer.SetPosition(0, T1.position + T1Offset);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, T2.position);

        if (hit && movement != null)
        {
            if (Vector3.Distance(movement.transform.position, T2.position) > maxRopeLenth
            && Vector3.Distance(movement.transform.position, T2.position) < Vector3.Distance(movement.transform.position + movement.velocity, T2.position))
            {
                Vector3 limitedPosition = (movement.transform.position - T2.position).normalized * maxRopeLenth + T2.position;
                movement.body.position = limitedPosition;
            }
        }

        hitTime += Time.deltaTime;
    }

    void BounceBackWithPath()
    {

        //prodjemo jos jednom kroz sve tacke kako bi dodali onaj efekat kao da uze viori i okrece u vazduhu
        //imas funkcije u desmos ako nes ne razumes klipice
        for (int i = 1; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 dir = (T2.position - T1.position + T1Offset).normalized;
            Vector3 offset = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), Mathf.Cos((i + Time.timeSinceLevelLoad * spinSpeed) / 4f), 0);

            lineRenderer.SetPosition(i,
            path[i - 1] + (offset
                    * kurvaBounceBack.Evaluate(bounceTime)//kurva
                    * Mathf.Min(((lineRenderer.positionCount - 1) / 2f - Mathf.Abs(i - (lineRenderer.positionCount - 1) / 2f)) / 19f, 1)));

        }
        //        Debug.Log(kurvaBounceBack.Evaluate(bounceTime));
        bounceTime += Time.deltaTime;
    }

    void AddPathLink(Vector3 point)
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
        bounceTime = 0;
        returnToSender = false;
        reachedMaxRopeLenth = false;
        combinedLenth = 0;
        path.Clear();
        pathDistances.Clear();
    }

    Vector3 PointAtRatio(Vector3 p1, Vector3 p2, float r1, float r2)
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
        PlayerMovement mTmp = T1.gameObject.GetComponent<PlayerMovement>();
        if (mTmp != null)
        {
            movement = mTmp;
        }
    }

    public void ReleaseTransformsToFollow()
    {
        movement = null;
        T1 = null;
        T2 = null;
    }

    bool AreVectorsSimilar(Vector3 vector1, Vector3 vector2)
    {
        // Normalize the vectors to ensure they have a length of 1
        vector1.Normalize();
        vector2.Normalize();

        // Calculate the dot product between the two vectors
        float dotProduct = Vector3.Dot(vector1, vector2);

        // Calculate the angle in radians between the vectors using the dot product
        float angle = Mathf.Acos(dotProduct);

        // Convert the angle from radians to degrees
        float angleDegrees = Mathf.Rad2Deg * angle;

        // Check if the angle is less than or equal to 60 degrees
        return angleDegrees <= 60f;
    }
    /*
        void OnDrawGizmos()
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
