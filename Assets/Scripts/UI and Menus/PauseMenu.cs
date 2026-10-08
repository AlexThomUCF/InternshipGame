using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    [Header("Inventory")]
    [SerializeField] private InventoryController inventoryController;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Player.Pause.performed +=
            OnPause;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void OnPause(
        InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;

        // Stop inventory scrolling while paused.
        if (inventoryController != null)
        {
            inventoryController.IsPaused = true;
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        // Allow inventory scrolling again.
        if (inventoryController != null)
        {
            inventoryController.IsPaused = false;
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();
    }

    // Reloads scene and unfreezes game
    public void Retry()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        Time.timeScale = 1f;
    }
}