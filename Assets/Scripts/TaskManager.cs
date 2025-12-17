using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class TaskManager : MonoBehaviour
{
    public GameObject [] npcAgents;
    public TaskList taskList;
    public GameObject currentObject;
    public bool hasObjectTask = true;
   
    


    // Start is called before the first frame update
    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
        currentObject = this.gameObject;
    }

    void RemoveTask(GameObject obj)
    {
        taskList.RemoveImposterTask(obj);
    }

    void OnTriggerEnter(Collider other)
    {
       NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
       NPCAnimations npcAnimations = other.GetComponent<NPCAnimations>();
        AINavigation aiCheck = other.GetComponent<AINavigation>();
        if(agent != null)
        {
            //Debug.Log("Agent in area");
            if(agent.CompareTag("IMPOSTER") && taskList.tasksListAmount.Contains(currentObject) && aiCheck.isPerformingAction) // Need to add bool to check if they are just in the area or if they are in the area and emoting
            {
               // Debug.Log("This is an Imposter");
                taskList.tasksListAmount.Remove(currentObject);

                hasObjectTask = false;

                StartCoroutine(npcAnimations.TaskAnimations(agent));

                RemoveTask(currentObject);
 
                //For Jen/Imposter only, stop her, remove her from going to this task, play animation, resume her route, once list is empty she wins the game.
            }
            if(agent.CompareTag("NPC"))
            {
              //   agent.isStopped = true;
              //  Debug.Log("This is a NPC");
                

                StartCoroutine(npcAnimations.TaskAnimations(agent));
                //play animation

               // Debug.Log("This is " + this.name);
            }
        }
    }


}
