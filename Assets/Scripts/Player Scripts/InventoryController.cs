using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Item Sets")]
    [SerializeField] private GameObject[] itemSets;

    [Header("Crown")]
    [SerializeField] private CrownPickup crownPickup;

    [Header("Equip Animation")]
    [SerializeField] private float equipSpeed = 2f;

    private PlayerControls controls;

    private int currentItem = 0;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.InventoryScroll.performed +=
            OnInventoryScroll;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        SelectItem(0);
    }

    private void OnInventoryScroll(
        UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 scroll =
            context.ReadValue<Vector2>();

        // Mouse wheel DOWN = next item
        if (scroll.y < 0f)
        {
            NextItem();
        }

        // Mouse wheel UP = previous item
        else if (scroll.y > 0f)
        {
            PreviousItem();
        }
    }

    private void NextItem()
    {
        if (itemSets.Length == 0)
            return;

        int nextItem =
            currentItem;

        for (int i = 0;
             i < itemSets.Length;
             i++)
        {
            nextItem++;

            if (nextItem >= itemSets.Length)
            {
                nextItem = 0;
            }

            if (IsItemAvailable(nextItem))
            {
                SelectItem(nextItem);
                return;
            }
        }
    }

    private void PreviousItem()
    {
        if (itemSets.Length == 0)
            return;

        int previousItem =
            currentItem;

        for (int i = 0;
             i < itemSets.Length;
             i++)
        {
            previousItem--;

            if (previousItem < 0)
            {
                previousItem =
                    itemSets.Length - 1;
            }

            if (IsItemAvailable(previousItem))
            {
                SelectItem(previousItem);
                return;
            }
        }
    }

    private bool IsItemAvailable(int index)
    {
        // Slot 4 = Crown
        if (index == 3)
        {
            return crownPickup != null &&
                   crownPickup.isPickedUp;
        }

        // First three items are always available
        return true;
    }

    private void SelectItem(int index)
    {
        if (itemSets.Length == 0)
            return;

        if (!IsItemAvailable(index))
            return;

        for (int i = 0;
             i < itemSets.Length;
             i++)
        {
            if (itemSets[i] == null)
                continue;

            if (i == index)
            {
                // Enable selected item
                itemSets[i].SetActive(true);

                // Start below the camera
                Vector3 startPosition =
                    itemSets[i]
                    .transform
                    .localPosition;

                startPosition.y = -1f;

                itemSets[i]
                    .transform
                    .localPosition =
                    startPosition;

                // Move item upward
                StartCoroutine(
                    MoveItemUp(itemSets[i])
                );
            }
            else
            {
                // Hide all other items
                itemSets[i].SetActive(false);
            }
        }

        currentItem = index;
    }

    private System.Collections.IEnumerator MoveItemUp(
        GameObject item
    )
    {
        Transform itemTransform =
            item.transform;

        Vector3 targetPosition =
            itemTransform.localPosition;

        targetPosition.y = 0f;

        while (
            Mathf.Abs(
                itemTransform.localPosition.y -
                targetPosition.y
            ) > 0.01f
        )
        {
            itemTransform.localPosition =
                Vector3.MoveTowards(
                    itemTransform.localPosition,
                    targetPosition,
                    equipSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        itemTransform.localPosition =
            targetPosition;
    }
}