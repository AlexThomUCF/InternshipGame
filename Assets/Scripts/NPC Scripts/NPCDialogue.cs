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
    public float typingSpeed = 0.3f;
    public float displayTime = 3f;

    public int wordsPerPage = 12;


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
        {
            StopCoroutine(currentDialogue);
        }


        currentDialogue = StartCoroutine(
            TypeDialogue(speakerName, message));
    }



    IEnumerator TypeDialogue(string speakerName, string message)
    {
        IsDialoguePlaying = true;

        dialoguePanel.SetActive(true);

        nameText.text = speakerName;


        string[] words = message.Split(' ');

        string currentPage = "";
        int wordCount = 0;


        dialogueText.text = "";


        foreach (string word in words)
        {
            currentPage += word + " ";
            wordCount++;


            dialogueText.text = currentPage;


            yield return new WaitForSeconds(typingSpeed);


            if (wordCount >= wordsPerPage)
            {
                yield return new WaitForSeconds(1f);

                dialogueText.text = "";

                currentPage = "";
                wordCount = 0;
            }
        }


        yield return new WaitForSeconds(displayTime);


        dialoguePanel.SetActive(false);

        IsDialoguePlaying = false;
    }
}