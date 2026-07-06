using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCInterrogation : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Villager";

    [Header("Dialogue")]
    [TextArea]
    public string fillerDialogue = "Hello there! I was just walking around minding my own business.";

    private NavMeshAgent agent;
    private Animator animator;

    private bool beingQuestioned = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void Interrogate(Transform player)
    {
        if (beingQuestioned)
            return;

        StartCoroutine(QuestionRoutine(player));
    }

    IEnumerator QuestionRoutine(Transform player)
    {
        beingQuestioned = true;

        // Stop movement
        agent.isStopped = true;

        // Stop walking animation
        animator.SetBool("isMoving", false);

        // Face the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Show dialogue
        NPCDialogue.Instance.ShowDialogue(npcName, fillerDialogue);

        // Wait while dialogue is visible
        yield return new WaitForSeconds(5f);

        // Resume movement
        agent.isStopped = false;

        beingQuestioned = false;
    }
}