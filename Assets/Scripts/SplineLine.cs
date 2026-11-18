
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class SplineLine : MonoBehaviour
{
    public SplineContainer spline;
    public int resolution = 100;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        
    }

    private void LateUpdate()
    {
        DrawSpline();
    }

    void DrawSpline()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = resolution + 1;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = spline.EvaluatePosition(t);

            lineRenderer.SetPosition(i, point);
        }
    }
}
