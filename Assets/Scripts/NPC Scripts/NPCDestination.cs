using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDestination : MonoBehaviour
{
    [Tooltip("Name of the animation trigger to play when NPC arrives")]
    public string animationTrigger;

    [Header("Task Object")]
    public GameObject taskObjectPrefab;

    public string handBoneName = "RightHand";


}
