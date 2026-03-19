using UnityEngine;

public class CrownPickup : MonoBehaviour
{
    public bool isPickedUp = false;

    public Vector3 offset = new Vector3(0, 2f, 0);
    public float followSpeed = 10f;
    public float rotationSpeed = 100f;

    private Transform player;

    public void PickUp(Transform playerTransform)
    {
        player = playerTransform;
        isPickedUp = true;

        // Disable physics if it has any
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    void Update()
    {
        if (!isPickedUp || player == null) return;

        // Smoothly move above player
        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // Rotate crown
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
