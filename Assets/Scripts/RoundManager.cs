
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private int wrongCatches = 0;
    private bool winSequenceStarted = false;

    [Header("UI")]
    [SerializeField] private Image[] lifeIcons;
    [SerializeField] private GameObject loseMenu;
    [SerializeField] private GameObject winMenu;

    [Header("Win Cutscene")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private NetGun netGun;
    [SerializeField] private Behaviour playerCameraInput;

    [SerializeField] private float cameraPanTime = 1.5f;

    private void Awake()
    {
        Instance = this;

        if (winMenu != null)
        {
            winMenu.SetActive(false);
        }
    }

    public void LoseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
        loseMenu.SetActive(true);
    }

    public void OnCharacterCaught(
        GameObject character,
        bool wasImposter)
    {
        if (wasImposter)
        {
            Debug.Log(
                "Imposter caught! Starting trickster capture sequence!"
            );
        }
        else
        {
            wrongCatches++;

            Debug.Log(
                $"Wrong character caught. Total mistakes: {wrongCatches}"
            );

            // Hide a life icon.
            if (wrongCatches <= lifeIcons.Length)
            {
                lifeIcons[wrongCatches - 1].enabled = false;
            }

            if (wrongCatches >= 3)
            {
                Debug.Log(
                    "Player lost! Too many wrong catches."
                );

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0;
                loseMenu.SetActive(true);
            }
        }
    }

    public void StartWinSequence(GameObject trickster)
    {
        if (winSequenceStarted)
            return;

        winSequenceStarted = true;

        StartCoroutine(WinSequence(trickster));
    }

    private IEnumerator WinSequence(GameObject trickster)
    {
        // Freeze player movement.
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Prevent shooting.
        if (netGun != null)
        {
            netGun.enabled = false;
        }

        // Stop camera look input, if assigned.
        if (playerCameraInput != null)
        {
            playerCameraInput.enabled = false;
        }

        // Find the Cinemachine camera attached to the trickster.
        CinemachineVirtualCamera cutsceneCamera = null;

        if (trickster != null)
        {
            cutsceneCamera =
                trickster.GetComponentInChildren<CinemachineVirtualCamera>();

            if (cutsceneCamera != null)
            {
                cutsceneCamera.Priority = 100;
            }
            else
            {
                Debug.LogWarning(
                    "No Cinemachine Virtual Camera found on trickster prefab!"
                );
            }
        }

        // Allow Cinemachine to blend to the cutscene camera.
        yield return new WaitForSeconds(cameraPanTime);

        // Display dialogue.
        if (NPCDialogue.Instance != null)
        {
            NPCDialogue.Instance.ShowDialogue(
                "Temp text. Temp text. Temp text. Temp text. Temp text."
            );

            // Wait for dialogue to finish.
            while (NPCDialogue.Instance.IsDialoguePlaying)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning(
                "NPCDialogue instance could not be found."
            );
        }

        // Show the Win Menu.
        if (winMenu != null)
        {
            winMenu.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}
