using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask interactLayer;
    public LayerMask characterLayer;

    public TMP_Text promptText;

    private CrownPickup nearbyCrown;
    private CrownPickup currentCrown;
    public Transform crownAnchor;

    void Update()
    {
        // Crown pickup prompt
        if (nearbyCrown != null && !nearbyCrown.isPickedUp)
        {
            promptText.text = "Press E to pick up";
            return;
        }

        // Character interaction
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & characterLayer) != 0)
            {
                if (currentCrown != null && currentCrown.isPickedUp)
                {
                    promptText.text = "Press E to question";
                    return;
                }
            }
        }

        promptText.text = "";
    }

    public void SetNearbyCrown(CrownPickup crown)
    {
        nearbyCrown = crown;
    }

    public void ClearNearbyCrown(CrownPickup crown)
    {
        if (nearbyCrown == crown)
            nearbyCrown = null;
    }

    public void SetCurrentCrown(CrownPickup crown)
    {
        currentCrown = crown;
    }

    // This gets called by the Input System
    public void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Pickup crown
        if (nearbyCrown != null && !nearbyCrown.isPickedUp)
        {
            nearbyCrown.PickUp(crownAnchor);
            currentCrown = nearbyCrown;
            promptText.text = "";
            return;
        }

        // Use crown on character
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & characterLayer) != 0)
            {
                if (currentCrown != null && currentCrown.isPickedUp)
                {
                    currentCrown.UseCrown();
                    promptText.text = "";
                }
            }
        }
    }
}
