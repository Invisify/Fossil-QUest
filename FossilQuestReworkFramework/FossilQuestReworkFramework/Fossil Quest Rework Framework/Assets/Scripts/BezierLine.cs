using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class BezierLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public Vector3 middlePoint;

    public int vertexCount;

    public float curveAmount = .5f;

    Vector3 startPos;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        CalculateLine();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLine();
    }

    private void CalculateLine()
    {
        //startPos = startPoint.position - startPoint.transform.GetChild(0).transform.up * 0.15f;
        //startPos = startPoint.position - startPoint.transform.up * 0.15f;
        startPos = startPoint.position;
        
        middlePoint = (endPoint.position + startPos) / 2;

        //Vector3.Reflect()

        Vector3 normalizedDir = middlePoint.normalized;
        Vector3 oppositeDir = new Vector3(normalizedDir.y, normalizedDir.x, normalizedDir.z);

        //middlePoint -= oppositeDir * 0.5f;
        

        middlePoint -= startPoint.transform.up * curveAmount;
        //middlePoint -= Vector3.up * 0.5f;

        List<Vector3> pointList = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPos, middlePoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(middlePoint, endPoint.position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);

            pointList.Add(bezierPoint);
        }

        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPoint.position, middlePoint);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(middlePoint, endPoint.position);

        Gizmos.color = Color.red;
        for (float ratio =  0.5f/ vertexCount; ratio < 1; ratio += 1f/ vertexCount)
        {
            Gizmos.DrawLine(Vector3.Lerp(startPos, middlePoint, ratio),
                Vector3.Lerp(middlePoint, endPoint.position, ratio));
        }
    }
}
