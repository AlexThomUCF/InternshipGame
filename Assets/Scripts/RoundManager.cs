using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private int wrongCatches = 0; // Track wrong NPCs

    [Header("UI")]
    [SerializeField] private Image[] lifeIcons; // Assign 3 life images in inspector

    private void Awake()
    {
        Instance = this;
    }

    // This gets called when any character is caught
    public void OnCharacterCaught(GameObject character, bool wasImposter)
    {
        if (wasImposter)
        {
            Debug.Log("Imposter caught! Player wins!");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen");
        }
        else
        {
            wrongCatches++;
            Debug.Log($"Wrong character caught. Total mistakes: {wrongCatches}");

            // Hide a life icon
            if (wrongCatches <= lifeIcons.Length)
            {
                lifeIcons[wrongCatches - 1].enabled = false;
            }

            if (wrongCatches >= 3)
            {
                Debug.Log("Player lost! Too many wrong catches.");

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }
    }
}


