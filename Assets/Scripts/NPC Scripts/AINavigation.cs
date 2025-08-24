using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    public NavMeshAgent myAgent;
    public float range; //Radius of spehere around agent. 
    public Transform location; 
    public Transform centrePoint; // centre of the area the agent wants to move around in
    public GameObject [] taskCheckpoints;
    public  int choice = 0;
    public Animator animator;

    public TaskList taskList;

    public bool isPerformingAction = false;

    // Start is called before the first frame update
    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
        //myAgent.SetDestination(location.position);
        
        taskCheckpoints = taskList.taskArray;   
    }

    // Update is called once per frame
    void Update()
    {
       // StartCoroutine(ChooseAction());
        if(myAgent.remainingDistance <= myAgent.stoppingDistance) // done with path
        {
            ChooseAction(); // when path is done call Choose action command
            Vector3 point;

            if(RandomPoint(centrePoint.position, range, out point) && (choice >= 41 && choice <= 100)) // If Random point is in range and choice is between 3 and 10, Move to random point on map
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                myAgent.SetDestination(point);
                StartCoroutine(ResetAfterMovement());
            }
        }
        
        if(CompareTag("IMPOSTER"))
        {
            taskCheckpoints = taskList.imposterTaskArray;// removes task from imposter array. The Imposter whill no longer go to this task
        }
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
            Debug.Log(gameObject.name + " choice: " + choice);

            //After these choices were picked instantly new choices were picked and maybe overrided this line. THIS IS CORRECT WHENEVER NUMBER IS BETWEEN 21 - 40 IT MAKES CHOICE PICK RIGHT AWAY SKIPPING THIS STAND STILL LINE.

            StartCoroutine(PauseMovement(4.4f));

        }
        else if (choice >= 41 && choice <= 100) // if choice 3 - 10 player free roams
        {
            Debug.Log(gameObject.name + " choice: " + choice);
            isPerformingAction = true;
            animator.SetBool("isMoving", true);

           // Debug.Log("Agent is roaming");

            
            //free roam 
        }
        else if(choice >= 1 && choice <= 20) // if choice 2 player moves to task point
        {
            isPerformingAction = true;
            Debug.Log(gameObject.name + " choice: " + choice);
            animator.SetBool("isMoving", true);

            int arrayLength = taskCheckpoints.Length;
            // go to task 
            int tempNum  = Random.Range(1,arrayLength);

            switch (tempNum) 
            {
                case 1:
                myAgent.SetDestination(taskCheckpoints[0].transform.position);
                float dist = Vector3.Distance(taskCheckpoints[0].transform.position, transform.position);   
                //yield return new WaitForSeconds(dist);
                break;

                case 2:
                myAgent.SetDestination(taskCheckpoints[1].transform.position);
                dist = Vector3.Distance(taskCheckpoints[1].transform.position, transform.position);   
                //yield return new WaitForSeconds(dist);
                break;

                case 3:
                myAgent.SetDestination(taskCheckpoints[2].transform.position);
                dist = Vector3.Distance(taskCheckpoints[2].transform.position, transform.position);   
               // yield return new WaitForSeconds(dist);
                break;
                
                case 4:
                myAgent.SetDestination(taskCheckpoints[3].transform.position);
                dist = Vector3.Distance(taskCheckpoints[3].transform.position, transform.position);   
                //yield return new WaitForSeconds(dist);
                break;
            }
            StartCoroutine(ResetAfterMovement());
        }
       //yield return new WaitForSeconds(2f);
    }

    IEnumerator PauseMovement(float pauseTime)
    {
        isPerformingAction = true;
        myAgent.isStopped = true;
        animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(pauseTime);

        myAgent.isStopped = false;
        animator.SetBool("isMoving", true);

        isPerformingAction = false;
    }

    

    IEnumerator ResetAfterMovement()
    {
        while (myAgent.pathPending || myAgent.remainingDistance > myAgent.stoppingDistance)
        {
            yield return null; // Wait until the AI reaches its destination
        }
        isPerformingAction = false;
    }
 
}
