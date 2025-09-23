using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScript : MonoBehaviour
{
    public TaskList taskList;

    // Start is called before the first frame update
    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
    }

    // Update is called once per frame
    void Update()
    {
        GameLost();
    }

    void GameLost()
    {
        if(taskList.imposterTaskArray.Length <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
