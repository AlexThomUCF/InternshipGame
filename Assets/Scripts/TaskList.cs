using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
    public List<GameObject> tasksList;
    public GameObject[] taskArray;
    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < taskArray.Length; i++)
        {
            tasksList.Add(taskArray[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
