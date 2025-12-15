using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xray : MonoBehaviour
{
    private List<GameObject> xRayList;
    private TaskList taskList;
    public int xRayLayer;
    public int defaultLayer;
    private PlayerControls controls;
    private GameObject lastSeenTask;
    //Needs a bar to track how long ability is 
    //float to track how much it drains a second and how long it takes to recharge ability
    // Needs to track what tasks are considered done
    // toggle if active or not
    // Start is called before the first frame update
    private void Awake()
    {
        controls = new PlayerControls();
    }
    void Start()
    {
        taskList = FindFirstObjectByType<TaskList>();
        xRayList = new List<GameObject>();

    }
    void OnEnable()
    {
        controls.Enable();
        controls.Player.Xray.started += ctx => StartXray();
        controls.Player.Xray.canceled += ctx => StopXray();
    }

    void OnDisable()
    {
        controls.Player.Xray.started -= ctx => StartXray();
        controls.Player.Xray.canceled -= ctx => StopXray();
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        addTask();
      

    }

    public void StartXray()
    {
        Debug.Log($"XRay START — Objects: {xRayList.Count}");

        foreach (GameObject obj in xRayList)
        {
            {
                if (obj == null) continue;


                Debug.Log($"Setting layer on {obj.name}");
                SetLayerRecursively(obj, xRayLayer);

            }
        }
    }
    
    public void StopXray()
    {
        foreach (GameObject obj in xRayList)
        {
            {
                if (obj == null) continue;

                SetLayerRecursively(obj, defaultLayer);

            }
        }
    }
    private void addTask()
    {
        if (taskList == null) return;

        GameObject task = taskList.lastRemovedTask;

        if (task == null || task == lastSeenTask) return;


        lastSeenTask = task;
        xRayList.Add(task);

    }
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
}
