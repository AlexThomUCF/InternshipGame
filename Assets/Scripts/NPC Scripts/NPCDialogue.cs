using System.Collections;
using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public static NPCDialogue Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.03f;
    public float displayTime = 3f;

    private Coroutine currentDialogue;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string speakerName, string message)
    {
        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(TypeDialogue(speakerName, message));
    }

    IEnumerator TypeDialogue(string speakerName, string message)
    {
        dialoguePanel.SetActive(true);

        nameText.text = speakerName;
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