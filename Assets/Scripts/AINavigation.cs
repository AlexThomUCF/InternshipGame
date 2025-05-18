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

    // Start is called before the first frame update
    void Start()
    {
        //myAgent.SetDestination(location.position);
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(ChooseAction());
        if(myAgent.remainingDistance <= myAgent.stoppingDistance) // done with path
        {
            Vector3 point;
            if(RandomPoint(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                myAgent.SetDestination(point);
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

    IEnumerator ChooseAction()
    {
        int choice = 0;
        choice = Random.Range(1,4);
        yield return new WaitForSeconds(5f);
        if(choice == 1)
        {
            Debug.Log(choice);
            //Agent stands still
            if(myAgent.remainingDistance >= myAgent.stoppingDistance)
            {
                myAgent.ResetPath();
            }

        }
        else if(choice == 2)
        {
            Debug.Log(choice);
            //free roam 
        }
        else if(choice == 3)
        {
            Debug.Log(choice);
            // do action
        }

       yield return new WaitForSeconds(2f);
    }
    
}
