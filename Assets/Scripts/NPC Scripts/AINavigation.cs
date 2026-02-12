using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    //isPerformingAction, stays true while npc is moving towards task point. Any task passed while true will be marked as completed. Need to change it so isPerforming task is activated once at task npc needs to 
    //go to
    public NavMeshAgent myAgent;
    public Animator animator;
    public float range; //Radius of spehere around agent. 
    public Transform location; 
    public Transform centrePoint; // centre of the area the agent wants to move around in
    public GameObject [] taskCheckpoints;
    public  int choice = 0;

    private NPCDestination currentDestination;
    private GameObject currentTaskTarget;


    public TaskList taskList;

    public bool isPerformingAction = false;
    

    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
        animator = GetComponent<Animator>();
        //myAgent.SetDestination(location.position);
        
        taskCheckpoints = taskList.taskArray;   
    }

    // Update is called once per frame
    void Update()
    {
       // StartCoroutine(ChooseAction());
        if(!isPerformingAction && !myAgent.pathPending && myAgent.remainingDistance <= myAgent.stoppingDistance) // done with path
        {
            ChooseAction(); // when path is done call Choose action command
        }
        
        if(CompareTag("IMPOSTER"))
        {
            taskCheckpoints = taskList.imposterTaskArray;// removes task from imposter array. The Imposter whill no longer go to this task
        }

        MovementAnimations();
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in sphere
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;   
    }

    public void ChooseAction()
    {
        if(isPerformingAction) return;
        choice = Random.Range(1,101);
       
        if(choice >= 21 && choice <= 40) // If choice 1 player stand still
        {
            //Debug.Log(gameObject.name + " choice: " + choice);

            //After these choices were picked instantly new choices were picked and maybe overrided this line. THIS IS CORRECT WHENEVER NUMBER IS BETWEEN 21 - 40 IT MAKES CHOICE PICK RIGHT AWAY SKIPPING THIS STAND STILL LINE.

            StartCoroutine(PauseMovement(4.4f));

        }
        else if (choice >= 41 && choice <= 100) // if choice 3 - 10 player free roams
        {
            isPerformingAction = true;
            moving = true;

            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
            {
                myAgent.SetDestination(point);
                StartCoroutine(ResetAfterMovement());
            }
            else
            {
                isPerformingAction = false;
                moving = false;
            }
            //free roam 
        }
        else if(choice >= 1 && choice <= 20) // if choice 2 player moves to task point
        {
            isPerformingAction = true;
           // Debug.Log(gameObject.name + " choice: " + choice);
            moving = true; // NEW ANIMATION IS MOVING TRIGGER

            int arrayLength = taskCheckpoints.Length;
            // go to task 
            int tempNum = Random.Range(0, arrayLength);

            string name1 = taskCheckpoints[tempNum].name;

            if(gameObject.tag == "IMPOSTER")
            {
                Debug.Log("Agent is going towards" + name1);
            }


            currentTaskTarget = taskCheckpoints[tempNum];
            myAgent.SetDestination(currentTaskTarget.transform.position);

            StartCoroutine(ResetAfterMovement());

        }
       //yield return new WaitForSeconds(2f);
    }

    IEnumerator PauseMovement(float pauseTime)
    {
        isPerformingAction = true;
        myAgent.isStopped = true;
        moving = false; // NEW ANIMATION IS MOVING TRIGGER


        yield return new WaitForSeconds(pauseTime);

        myAgent.isStopped = false;
        moving = true; // NEW ANIMATION IS MOVING TRIGGER

        isPerformingAction = false;
        
    }

    

    IEnumerator ResetAfterMovement()
    {
        while (myAgent.pathPending || myAgent.remainingDistance > myAgent.stoppingDistance)
        {
            yield return null; // Wait until the AI reaches its destination
        }
        moving = false; // stop walking animation

        if (currentTaskTarget != null)
        {
            NPCDestination dest = currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null && dest.animationTrigger != "")
            {
                animator.SetTrigger(dest.animationTrigger);
            }

            currentTaskTarget = null;
        }



        isPerformingAction = false;
        
    }

    public void MovementAnimations()
    {
        if (moving == true)
        {
            animator.SetBool("isMoving", true);
        }
        else if (moving == false)
        {
            animator.SetBool("isMoving", false);
        }
    }


}
