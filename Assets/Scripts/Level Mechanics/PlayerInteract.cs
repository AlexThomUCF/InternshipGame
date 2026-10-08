using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask interactLayer;
    public LayerMask characterLayer;

    public TMP_Text promptText;

    [Header("Inventory")]
    [SerializeField] private InventoryController inventoryController;

    private CrownPickup nearbyCrown;
    private CrownPickup currentCrown;

    public Transform crownAnchor;

    void Update()
    {
        // Crown pickup prompt
        if (nearbyCrown != null &&
            !nearbyCrown.isPickedUp)
        {
            promptText.text =
                "Press E to pick up";

            return;
        }

        // Character interaction
        Ray ray = new Ray(
            transform.position,
            transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactDistance))
        {
            if (((1 << hit.collider.gameObject.layer) &
                 characterLayer) != 0)
            {
                // Only show the question prompt if
                // the player has the crown AND has
                // it selected.
                if (currentCrown != null &&
                    currentCrown.isPickedUp &&
                    inventoryController != null &&
                    inventoryController.IsCrownSelected)
                {
                    promptText.text =
                        "Press E to question";

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
        {
            nearbyCrown = null;
        }
    }

    public void SetCurrentCrown(CrownPickup crown)
    {
        currentCrown = crown;
    }

    public void OnInteract(
        UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (NPCDialogue.Instance != null &&
            NPCDialogue.Instance.IsDialoguePlaying)
        {
            return;
        }

        // -----------------------------------------
        // PICK UP CROWN
        // -----------------------------------------

        if (nearbyCrown != null &&
            !nearbyCrown.isPickedUp)
        {
            nearbyCrown.PickUp(crownAnchor);

            currentCrown =
                nearbyCrown;

            promptText.text = "";

            return;
        }

        // -----------------------------------------
        // QUESTION NPC
        // -----------------------------------------

        // Make sure the crown is actually selected
        // BEFORE doing anything with it.
        if (inventoryController == null)
            return;

        if (!inventoryController.IsCrownSelected)
            return;

        if (currentCrown == null ||
            !currentCrown.isPickedUp)
        {
            return;
        }

        Ray ray = new Ray(
            transform.position,
            transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactDistance))
        {
            if (((1 << hit.collider.gameObject.layer) &
                 characterLayer) != 0)
            {
                NPCInterrogation npc =
                    hit.collider.GetComponentInParent<
                        NPCInterrogation>();

                if (npc != null)
                {
                    // Only consume the crown after
                    // we've confirmed the crown is selected
                    // and we actually found an NPC.
                    currentCrown.UseCrown();

                    promptText.text = "";

                    npc.Interrogate(transform);
                }
            }
        }
    }
}