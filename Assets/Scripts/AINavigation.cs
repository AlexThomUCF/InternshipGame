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

    // Start is called before the first frame update
    void Start()
    {
        //myAgent.SetDestination(location.position);
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
                StartCoroutine(PauseMovement(4.4f)); // Maybe keep maybe delete
            }
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
        choice = Random.Range(0,100);
       
        if(choice >= 21 && choice <= 40) // If choice 1 player stand still
        {
            Debug.Log(gameObject.name + " choice: " + choice);

            StartCoroutine(PauseMovement(4.4f));

        }
        else if (choice >= 41 && choice <= 100) // if choice 3 - 10 player free roams
        {
            Debug.Log(gameObject.name + " choice: " + choice);

            Debug.Log("Agent is roaming");

            
            //free roam 
        }
        else if(choice >= 1 && choice <= 20) // if choice 2 player moves to task point
        {
            Debug.Log(gameObject.name + " choice: " + choice);

            // go to task 
            int tempNum  = Random.Range(1,4);

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
        }
       //yield return new WaitForSeconds(2f);
    }

    IEnumerator PauseMovement(float pauseTime)
    {
        myAgent.isStopped = true;

        yield return new WaitForSeconds(pauseTime);

        myAgent.isStopped = false;
    }

    // I feel like 2 is being selected to frequently, its like if one of the agent is going towards 2, the other ones go towards 2
    
}
