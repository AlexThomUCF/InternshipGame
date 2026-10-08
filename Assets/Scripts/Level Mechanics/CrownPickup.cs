using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class CrownPickup : MonoBehaviour
{
    public bool isPickedUp = false;

    private bool isRespawning = false;

    public float rotationSpeed = 100f;
    public float respawnTime = 60f;

    [Header("Crown Hand Size")]
    [SerializeField] private float handScale = 0.25f;

    // Original world position
    private Vector3 startPosition;

    // Original world rotation
    private Quaternion startRotation;

    // Original world scale
    private Vector3 startScale;

    private Transform anchor;

    private PlayerInteract playerInRange;

    private Renderer[] renderers;
    private Collider[] colliders;

    public VisualEffect poofVFX;
    public VisualEffect poofVFX2;

    // Pickup sound
    public AudioClip pickupSound;

    // Automatically created AudioSource
    private AudioSource audioSource;

    private void Start()
    {
        // Remember exactly where the crown starts.
        startPosition =
            transform.position;

        startRotation =
            transform.rotation;

        startScale =
            transform.lossyScale;

        // Cache all renderers.
        renderers =
            GetComponentsInChildren<Renderer>();

        // Cache BOTH colliders.
        colliders =
            GetComponentsInChildren<Collider>();

        // Create/get AudioSource.
        audioSource =
            GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource =
                gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp || isRespawning)
            return;

        if (other.CompareTag("Player"))
        {
            playerInRange =
                other.GetComponent<PlayerInteract>();

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
        if (isPickedUp || isRespawning)
            return;

        isPickedUp = true;

        anchor = crownAnchor;

        // Disable BOTH colliders.
        SetCollidersEnabled(false);

        // Parent crown to player's hand.
        transform.SetParent(anchor);

        transform.localPosition =
            Vector3.zero;

        transform.localRotation =
            Quaternion.identity;

        // Make crown smaller in player's hand.
        transform.localScale =
            Vector3.one * handScale;

        // Play pickup VFX.
        if (poofVFX != null)
        {
            poofVFX.Play();
        }

        // Play pickup sound.
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(
                pickupSound
            );
        }
    }

    public void UseCrown()
    {
        if (!isPickedUp)
            return;

        // Play use VFX.
        if (poofVFX2 != null)
        {
            poofVFX2.Play();
        }

        // Remove crown from player's hand.
        transform.SetParent(null);

        // Put crown back at its original position.
        transform.position =
            startPosition;

        transform.rotation =
            startRotation;

        // Restore original size.
        transform.localScale =
            startScale;

        // No longer held.
        isPickedUp = false;

        // Begin respawn cooldown.
        isRespawning = true;

        // Hide crown.
        SetVisibility(false);

        // Disable BOTH colliders.
        SetCollidersEnabled(false);

        // Clear PlayerInteract reference.
        if (playerInRange != null)
        {
            playerInRange.ClearNearbyCrown(this);
            playerInRange = null;
        }

        // Start cooldown.
        StartCoroutine(
            RespawnCrown()
        );
    }

    private IEnumerator RespawnCrown()
    {
        yield return new WaitForSeconds(
            respawnTime
        );

        // Crown is available again.
        isRespawning = false;

        // Make crown visible.
        SetVisibility(true);

        // Enable BOTH colliders.
        SetCollidersEnabled(true);

        // Play respawn VFX.
        if (poofVFX != null)
        {
            poofVFX.Play();
        }
    }

    private void SetCollidersEnabled(bool enabled)
    {
        if (colliders == null)
            return;

        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }

    private void SetVisibility(bool visible)
    {
        if (renderers == null)
            return;

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }

    private void Update()
    {
        if (!isPickedUp)
            return;

        transform.Rotate(
            Vector3.up *
            rotationSpeed *
            Time.deltaTime
        );
    }
}