using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Item Sets")]
    [SerializeField] private GameObject[] itemSets;

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
            if (itemSets[i] != null)
            {
                itemSets[i].SetActive(i == index);
            }
        }

        currentItem = index;
    }
}
