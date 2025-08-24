using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatchableCharacter : MonoBehaviour
{
    public NavMeshAgent agent;    // Assign NavMeshAgent
    public Animator animator;     // For possible future "Caught" animation
    public bool isCaught { get; private set; }

    public void Catch(bool wasImposter)
    {
        if (isCaught) return;
        isCaught = true;

        if (agent) agent.isStopped = true;
        if (animator) animator.SetTrigger("Caught"); // Create a "Caught" trigger

        // Notify round system
        RoundManager.Instance.OnCharacterCaught(gameObject, wasImposter);
    }
}
