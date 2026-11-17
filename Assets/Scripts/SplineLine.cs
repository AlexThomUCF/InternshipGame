
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class SplineLine : MonoBehaviour
{
    public SplineContainer spline;
    public int resolution = 100;
    public float animationSpeed = 2f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        StartCoroutine(AnimateLine());
    }

    IEnumerator AnimateLine()
    {
        lineRenderer.positionCount = 0;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 point = spline.EvaluatePosition(t);

            lineRenderer.positionCount = i + 1;
            lineRenderer.SetPosition(i, point);

            yield return new WaitForSeconds(1f / (resolution * animationSpeed));
        }
    }
}
