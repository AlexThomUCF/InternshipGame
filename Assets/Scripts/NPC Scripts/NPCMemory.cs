using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCMemory : MonoBehaviour
{
    [SerializeField]
    private List<string> completedTasks = new List<string>();


    public void AddCompletedTask(string taskName)
    {
        if (!completedTasks.Contains(taskName))
        {
            completedTasks.Add(taskName);
        }
    }


    public string GetTaskDialogue()
    {
        if (completedTasks.Count == 0)
        {
            return "I've just been walking around.";
        }


        List<string> randomTasks = new List<string>(completedTasks);


        for (int i = 0; i < randomTasks.Count; i++)
        {
            int randomIndex = Random.Range(i, randomTasks.Count);

            string temp = randomTasks[i];
            randomTasks[i] = randomTasks[randomIndex];
            randomTasks[randomIndex] = temp;
        }


        StringBuilder dialogue = new StringBuilder();

        dialogue.Append("I've completed ");

        for (int i = 0; i < randomTasks.Count; i++)
        {
            dialogue.Append(randomTasks[i].ToLower());


            if (i < randomTasks.Count - 2)
            {
                dialogue.Append(", ");
            }
            else if (i == randomTasks.Count - 2)
            {
                dialogue.Append(", and ");
            }
            else
            {
                dialogue.Append(".");
            }
        }


        return dialogue.ToString();
    }
}