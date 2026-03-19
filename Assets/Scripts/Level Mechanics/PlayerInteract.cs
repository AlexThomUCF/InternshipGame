using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask interactLayer;

    public TMP_Text promptText;

    private CrownPickup currentCrown;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            CrownPickup crown = hit.collider.GetComponentInParent<CrownPickup>();

            if (crown != null && !crown.isPickedUp)
            {
                promptText.text = "Press E to pick up";
                currentCrown = crown;
                return;
            }
        }

        // Nothing valid hit
        promptText.text = "";
        currentCrown = null;
    }

    //This gets called by the Input System
    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("INTERACT TRIGGERED");

        if (context.performed)
        {
            Debug.Log("INTERACT PERFORMED");

            if (currentCrown != null)
            {
                Debug.Log("Picking up crown");
                currentCrown.PickUp(transform);
                promptText.text = "";
            }
        }
    }
}
