using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCInterrogation : MonoBehaviour
{

    [Header("NPC Info")]
    public string npcName = "Villager";


    [Header("Dialogue")]
    [TextArea]
    public string fillerDialogue =
        "Hello there! I was just walking around minding my own business.";


    [Header("Rotation")]
    public float turnSpeed = 180f;


    private NavMeshAgent agent;
    private Animator animator;
    private AINavigation navigation;


    private bool beingQuestioned;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navigation = GetComponent<AINavigation>();
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


        navigation.isPaused = true;

        agent.isStopped = true;


        yield return StartCoroutine(LookAtPlayer(player));


        NPCDialogue.Instance.ShowDialogue(
            npcName,
            fillerDialogue
        );


        while (NPCDialogue.Instance.IsDialoguePlaying)
        {
            yield return null;
        }


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