using UnityEngine;
using System.Collections;

public class CrownPickup : MonoBehaviour
{
    public bool isPickedUp = false;

    public float rotationSpeed = 100f;
    public float respawnTime = 60f;

    private Transform anchor;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private PlayerInteract playerInRange;

    private Renderer[] renderers;
    private Collider col;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Cache renderer(s) and collider
        renderers = GetComponentsInChildren<Renderer>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = other.GetComponent<PlayerInteract>();

            if (playerInRange != null)
            {
                playerInRange.SetNearbyCrown(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInRange != null)
            {
                playerInRange.ClearNearbyCrown(this);
                playerInRange = null;
            }
        }
    }

    public void PickUp(Transform crownAnchor)
    {
        isPickedUp = true;
        anchor = crownAnchor;

        // Disable trigger so it doesn’t retrigger
        if (col != null) col.enabled = false;

        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void UseCrown()
    {
        if (!isPickedUp) return;

        // Hide crown visually instead of deactivating
        SetVisibility(false);

        isPickedUp = false;

        // Start cooldown coroutine
        StartCoroutine(RespawnCrown());
    }

    IEnumerator RespawnCrown()
    {
        yield return new WaitForSeconds(respawnTime);

        // Reset parent and position
        transform.SetParent(null);
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Show again and re-enable collider
        SetVisibility(true);
        if (col != null) col.enabled = true;
    }

    void SetVisibility(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    void Update()
    {
        if (!isPickedUp) return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
