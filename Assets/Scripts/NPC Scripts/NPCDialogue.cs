using System.Collections;
using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public static NPCDialogue Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.03f;
    public float displayTime = 3f;

    public bool IsDialoguePlaying { get; private set; }

    private Coroutine currentDialogue;

    private void Awake()
    {
        Instance = this;

        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string message)
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
        }

        currentDialogue = StartCoroutine(TypeDialogue(message));
    }

    private IEnumerator TypeDialogue(string message)
    {
        IsDialoguePlaying = true;

        dialoguePanel.SetActive(true);

        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(displayTime);

        dialoguePanel.SetActive(false);

        IsDialoguePlaying = false;

        currentDialogue = null;
    }
}