using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextUpdate : MonoBehaviour
{
    public TaskList taskList;
    public TextMeshProUGUI textDisplay;
    // Start is called before the first frame update
  
    void Start()    
    {
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

}
