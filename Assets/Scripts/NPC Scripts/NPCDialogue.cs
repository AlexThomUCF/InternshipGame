using System.Collections;
using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public static NPCDialogue Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    public float typingSpeed = 0.03f;
    public float displayTime = 3f;

    Coroutine currentDialogue;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string message)
    {
        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(TypeDialogue(message));
    }

    IEnumerator TypeDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(displayTime);

        dialoguePanel.SetActive(false);
    }
}