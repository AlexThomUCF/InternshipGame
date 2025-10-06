using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class Test : MonoBehaviour, IInteraction
{
    public UnityEvent onInteract { get; set; } = new UnityEvent();
    MeshRenderer mesh;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    TaskList task;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        task = FindAnyObjectByType<TaskList>();
    }

  

    public void Interact()
    {
        onInteract?.Invoke();
        StartCoroutine(wallySpeak());
        
    }

    IEnumerator wallySpeak()
    {
        if (task.lastRemovedTask == null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = ("I have nothing to tell you");
            yield return new WaitForSeconds(4f);
            dialoguePanel.SetActive(false);
        }
        else
        {
            string objName = task.lastRemovedTask.name;
            dialoguePanel.SetActive(true);
            dialogueText.text = ("You shoud check out the " + objName + "");
            yield return new WaitForSeconds(4f);
            dialoguePanel.SetActive(false);
        }


    }
}
