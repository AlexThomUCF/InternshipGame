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

    public void NPCReachedTask(AINavigation npc)
    {
        NavMeshAgent agent = npc.myAgent;

        if (agent.CompareTag("IMPOSTER") &&
            taskList.tasksListAmount.Contains(currentObject))
        {
            Debug.Log(agent.gameObject.name + " reached task: " + currentObject.name);

            taskList.tasksListAmount.Remove(currentObject);

            hasObjectTask = false;

            RemoveTask(currentObject);
        }
    }

   


}
