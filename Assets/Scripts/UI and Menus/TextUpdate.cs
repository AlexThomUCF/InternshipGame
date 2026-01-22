using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextUpdate : MonoBehaviour
{
    public TaskList taskList;
    public TextMeshProUGUI textDisplay;
    public GameObject taskUI;

    private bool isUIOpen;
    private PlayerControls controls;
    // Start is called before the first frame update

    private void Awake()
    {
        controls = new PlayerControls();
    }
    void Start()    
    {
        isUIOpen = taskUI.activeSelf;
        taskList = FindObjectOfType<TaskList>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateText();
    }
    void UpdateText()
    {
        string output = "";
        foreach (GameObject obj in taskList.imposterTaskArray)
        {
            if(obj != null)
            output += obj.name + "\n";
        }

        textDisplay.text = output;
    }
    void OnEnable()
    {
        controls.Enable();
        controls.Player.UIToggle.started += OnUIToggle;
    }

    void OnDisable()
    {
        controls.Player.UIToggle.started -= OnUIToggle;
        controls.Disable();
    }
    void ToggleUIOn()
    {   
        taskUI.SetActive(true);
    }
    void ToggleUIOff()
    {
        taskUI.SetActive(false);
    }

    void OnUIToggle(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        isUIOpen = !isUIOpen;
        taskUI.SetActive(isUIOpen);
    }

 


}
