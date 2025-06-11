using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskManager : MonoBehaviour
{
    public GameObject [] npcAgents;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
       NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            Debug.Log("Agent in area");
        }
    }
}
