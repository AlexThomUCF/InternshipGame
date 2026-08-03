using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCInterrogation : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Villager";


    [Header("Rotation")]
    public float turnSpeed = 180f;


    private NavMeshAgent agent;
    private Animator animator;
    private AINavigation navigation;
    private NPCMemory memory;


    private bool beingQuestioned;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navigation = GetComponent<AINavigation>();
        memory = GetComponent<NPCMemory>();
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


        // Pause AI
        if (navigation != null)
            navigation.isPaused = true;


        agent.isStopped = true;


        // Smoothly face player
        yield return StartCoroutine(LookAtPlayer(player));



        string dialogue;


        if (memory != null)
        {
            dialogue = memory.GetTaskDialogue();
        }
        else
        {
            dialogue = "I've just been walking around.";
        }



        NPCDialogue.Instance.ShowDialogue(
            npcName,
            dialogue
        );



        // Wait for dialogue to finish
        while (NPCDialogue.Instance.IsDialoguePlaying)
        {
            yield return null;
        }



        // Resume AI
        if (navigation != null)
            navigation.isPaused = false;


        agent.isStopped = false;


        beingQuestioned = false;
    }



    IEnumerator LookAtPlayer(Transform player)
    {
        Vector3 direction =
            player.position - transform.position;


        direction.y = 0;


        Quaternion targetRotation =
            Quaternion.LookRotation(direction);



        while (Quaternion.Angle(
            transform.rotation,
            targetRotation) > .5f)
        {
            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );


            yield return null;
        }
    }
}