using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskList : MonoBehaviour
{
    public List<GameObject> tasksListAmount;
    public GameObject[] taskArray;
    public GameObject[] imposterTaskArray; //seperate array to track imposters tasks

    // Start is called before the first frame update
    void Start()
    {
        imposterTaskArray = taskArray;
        for(int i = 0; i < taskArray.Length; i++)
        {
            tasksListAmount.Add(taskArray[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
