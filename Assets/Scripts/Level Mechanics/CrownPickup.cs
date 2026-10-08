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
    [SerializeField] private float handScale = 0.5f;

    [Header("Crown VFX Size")]
    [SerializeField] private float handVFXScale = 0.5f;

    private Transform anchor;

    // Original throne settings
    private Transform startParent;
    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private Vector3 startScale;

    // Original VFX scale
    private Vector3 poofVFXStartScale;

    private PlayerInteract playerInRange;

    private Renderer[] renderers;

    // Both crown colliders
    private Collider[] colliders;

    public VisualEffect poofVFX;
    public VisualEffect poofVFX2;

    // Pickup sound
    public AudioClip pickupSound;

    // Automatically created AudioSource
    private AudioSource audioSource;

    private void Start()
    {
        // Remember exactly where the crown starts on the throne.
        startParent = transform.parent;
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
        startScale = transform.localScale;

        // Remember original VFX size.
        if (poofVFX != null)
        {
            poofVFXStartScale =
                poofVFX.transform.localScale;
        }

        // Cache all renderers.
        renderers =
            GetComponentsInChildren<Renderer>();

        // Cache ALL colliders on the crown.
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
        // Prevent pickup while already holding it
        // or while it is respawning.
        if (isPickedUp || isRespawning)
            return;

        isPickedUp = true;

        anchor = crownAnchor;

        // Disable BOTH colliders while held.
        SetCollidersEnabled(false);

        // Move crown into player's hand.
        transform.SetParent(anchor);

        transform.localPosition =
            Vector3.zero;

        transform.localRotation =
            Quaternion.identity;

        // Make crown smaller in player's hand.
        transform.localScale =
            startScale * handScale;

        // Make pickup smoke smaller.
        if (poofVFX != null)
        {
            poofVFX.transform.localScale =
                poofVFXStartScale *
                handVFXScale;
        }

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

        // Move crown back to the throne.
        transform.SetParent(
            startParent
        );

        transform.localPosition =
            startLocalPosition;

        transform.localRotation =
            startLocalRotation;

        transform.localScale =
            startScale;

        // No longer held.
        isPickedUp = false;

        // Start cooldown.
        isRespawning = true;

        // Hide crown.
        SetVisibility(false);

        // Disable BOTH colliders.
        SetCollidersEnabled(false);

        // Clear PlayerInteract's reference.
        if (playerInRange != null)
        {
            playerInRange.ClearNearbyCrown(this);
            playerInRange = null;
        }

        // Restore VFX size.
        if (poofVFX != null)
        {
            poofVFX.transform.localScale =
                poofVFXStartScale;
        }

        // Start respawn timer.
        StartCoroutine(
            RespawnCrown()
        );
    }

    private IEnumerator RespawnCrown()
    {
        // Wait for the respawn timer.
        yield return new WaitForSeconds(
            respawnTime
        );

        // Crown is available again.
        isRespawning = false;

        // Show crown.
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