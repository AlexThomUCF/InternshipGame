using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NPCMemory : MonoBehaviour
{
    [SerializeField] private List<string> completedTasks = new List<string>();


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

        // Randomize order
        for (int i = 0; i < randomTasks.Count; i++)
        {
            int randomIndex = Random.Range(i, randomTasks.Count);

            string temp = randomTasks[i];
            randomTasks[i] = randomTasks[randomIndex];
            randomTasks[randomIndex] = temp;
        }


        StringBuilder dialogue = new StringBuilder();

        dialogue.Append("I've completed these tasks:\n");


        foreach (string task in randomTasks)
        {
            dialogue.Append("• " + task + "\n");
        }


        return dialogue.ToString();
    }
}