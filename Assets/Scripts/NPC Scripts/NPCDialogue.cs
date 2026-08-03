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


    [Header("Settings")]
    public float typingSpeed = .03f;
    public float displayTime = 3f;


    public bool IsDialoguePlaying { get; private set; }


    private Coroutine currentDialogue;


    void Awake()
    {
        Instance = this;
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
        IsDialoguePlaying = true;

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

        IsDialoguePlaying = false;
    }
}