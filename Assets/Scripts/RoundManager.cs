using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

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
            Debug.Log("Wrong character caught. Nothing happens or add penalty here.");
        }
    }
}
