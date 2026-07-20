using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target; // Player or camera pivot
    public float cameraDistance = 5f;
    public float minDistance = 1f;
    public float collisionRadius = 0.3f;
    public float smoothSpeed = 10f;

    private float currentDistance;

    void Start()
    {
        currentDistance = cameraDistance;
    }

    void LateUpdate()
    {
        Vector3 direction = (transform.position - target.position).normalized;

        float targetDistance = cameraDistance;

        // Sphere cast prevents clipping better than a raycast
        if (Physics.SphereCast(
            target.position,
            collisionRadius,
            direction,
            out RaycastHit hit,
            cameraDistance))
        {
            targetDistance = Mathf.Clamp(
                hit.distance,
                minDistance,
                cameraDistance
            );
        }

        // Smooth camera movement
        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            Time.deltaTime * smoothSpeed
        );

        transform.position = target.position + direction * currentDistance;
    }
}