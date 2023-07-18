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
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }
    // Update is called once per frame
    void Update()
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
}
