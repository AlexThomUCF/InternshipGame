using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TaskList : MonoBehaviour
{
    public List<GameObject> tasksListAmount;
    public GameObject[] taskArray;
    public GameObject[] imposterTaskArray; //seperate array to track imposters tasks
    public GameObject lastRemovedTask;     // store the last task removed

    // Start is called before the first frame update
    void Start()
    {
        imposterTaskArray = taskArray;
        for(int i = 0; i < taskArray.Length; i++)
        {
            tasksListAmount.Add(taskArray[i]);
        }
    }
    public void RemoveImposterTask(GameObject obj)
    {
        if (imposterTaskArray.Contains(obj))
        {
            lastRemovedTask = obj; // store the last one removed
            imposterTaskArray = imposterTaskArray.Where(g => g != obj).ToArray();
            Debug.Log("Last removed task (stored in TaskList): " + lastRemovedTask.name);
        }
    }
}
