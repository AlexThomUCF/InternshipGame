using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour, IInteraction
{
    public UnityEvent onInteract { get; set; } = new UnityEvent();

    MeshRenderer mesh;

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Interaction")]
    public GameObject interactionPrompt;

    [Header("Camera")]
    public CinemachineVirtualCamera wallyCamera;

    TaskList task;

    private bool playerInRange = false;
    private bool isTalking = false;


    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        task = FindAnyObjectByType<TaskList>();

        // Make sure the prompt and dialogue are hidden when the game starts
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Make sure Wally's camera starts inactive
        if (wallyCamera != null)
        {
            wallyCamera.Priority = 0;
        }
    }


    void Update()
    {
        // Only allow E to work when the player is close enough
        if (playerInRange && !isTalking)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Interact();
            }
        }
    }


    public void Interact()
    {
        if (isTalking)
            return;

        onInteract?.Invoke();

        StartCoroutine(wallySpeak());
    }


    IEnumerator wallySpeak()
    {
        isTalking = true;

        // Hide the "Press E to talk" prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Switch to Wally's camera
        if (wallyCamera != null)
        {
            wallyCamera.Priority = 20;
        }

        // Small delay so the camera has time to begin moving
        yield return new WaitForSeconds(0.5f);

        if (task.lastRemovedTask == null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "I have nothing to tell you";

            yield return new WaitForSeconds(4f);

            dialoguePanel.SetActive(false);
        }
        else
        {
            string objName = task.lastRemovedTask.name;

            dialoguePanel.SetActive(true);
            dialogueText.text = "You should check out the " + objName;

            yield return new WaitForSeconds(4f);

            dialoguePanel.SetActive(false);
        }

        // Switch back to the player's camera
        if (wallyCamera != null)
        {
            wallyCamera.Priority = 0;
        }

        // Give Cinemachine a moment to transition back
        yield return new WaitForSeconds(0.5f);

        isTalking = false;

        // If the player is still standing near Wally,
        // show the prompt again
        if (playerInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!isTalking && interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}