using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCInterrogation : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    bool beingQuestioned;

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

        agent.isStopped = true;

        // Stop walking animation
        animator.SetBool("isMoving", false);

        // Face player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;

        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookPos);

        NPCDialogue.Instance.ShowDialogue(
            "Hello there! I was just walking around minding my own business."
        );

        yield return new WaitForSeconds(5f);

        agent.isStopped = false;

        beingQuestioned = false;
    }
}