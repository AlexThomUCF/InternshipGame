using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Item Sets")]
    [SerializeField] private GameObject[] itemSets;

    [Header("Equip Animation")]
    [SerializeField] private float equipSpeed = 2f;

    private PlayerControls controls;

    private int currentItem = 0;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.InventoryScroll.performed += OnInventoryScroll;
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
        currentItem++;

        if (currentItem >= itemSets.Length)
        {
            currentItem = 0;
        }

        SelectItem(currentItem);
    }

    private void PreviousItem()
    {
        currentItem--;

        if (currentItem < 0)
        {
            currentItem = itemSets.Length - 1;
        }

        SelectItem(currentItem);
    }

    private void SelectItem(int index)
    {
        if (itemSets.Length == 0)
            return;

        for (int i = 0; i < itemSets.Length; i++)
        {
            if (itemSets[i] == null)
                continue;

            if (i == index)
            {
                // Turn the selected item on.
                itemSets[i].SetActive(true);

                // Make sure it starts below the camera.
                Vector3 startPosition =
                    itemSets[i].transform.localPosition;

                startPosition.y = -1f;

                itemSets[i].transform.localPosition =
                    startPosition;

                // Start the movement upward.
                StartCoroutine(
                    MoveItemUp(itemSets[i])
                );
            }
            else
            {
                // Hide all other items.
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
                    equipSpeed * Time.deltaTime
                );

            yield return null;
        }

        itemTransform.localPosition =
            targetPosition;
    }
}