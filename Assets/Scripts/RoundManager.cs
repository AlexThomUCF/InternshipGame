using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    private int wrongCatches = 0;

    [Header("UI")]
    [SerializeField] private Image[] lifeIcons;
    [SerializeField] private GameObject loseMenu;

    private void Awake()
    {
        Instance = this;
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

            // The transformation is handled by CatchableCharacter.
            // We'll add the win sequence later.
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
}