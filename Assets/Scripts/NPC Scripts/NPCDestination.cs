using UnityEngine;

public class NPCDestination : MonoBehaviour
{
    [Header("Task Information")]
    public string taskName = "Unnamed Task";

    [Tooltip("Name of the animation trigger to play when NPC arrives")]
    public string animationTrigger;


    [Header("Task Object")]
    public GameObject taskObjectPrefab;
    public HumanBodyBones attachBone;
}