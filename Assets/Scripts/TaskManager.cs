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

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void RemoveTask(GameObject obj)
    {
        if(!hasObjectTask)
        {
            taskList.imposterTaskArray = taskList.imposterTaskArray.Where(g => g != obj).ToArray();

        }
    }

    void OnTriggerEnter(Collider other)
    {
       NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            Debug.Log("Agent in area");
            if(agent.CompareTag("IMPOSTER") && taskList.tasksListAmount.Contains(currentObject)) // Need to add bool to check if they are just in the area or if they are in the area and emoting
            {
                Debug.Log("This is an Imposter");
                taskList.tasksListAmount.Remove(currentObject);

                hasObjectTask = false;

                RemoveTask(currentObject);

                //For Jen/Imposter only, stop her, remove her from going to this task, play animation, resume her route, once list is empty she wins the game.
            }
            if(agent.CompareTag("NPC"))
            {
                //agent.isStopped = true;
                Debug.Log("This is a NPC");
                //play animation
                //agent.isStopped = false;
                Debug.Log("This is " + this.name);
            }
        }
    }
}
