using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatchableCharacter : MonoBehaviour
{
    public NavMeshAgent agent;    // assign if you use NavMeshAgent
    public Animator animator;     // optional for a "Caught" animation
    public bool isCaught { get; private set; }

    public void Catch(bool wasImposter)
    {
        if (isCaught) return;
        isCaught = true;

        if (agent) agent.isStopped = true;
        if (animator) animator.SetTrigger("Caught"); // create a "Caught" trigger if you like

        // Notify round system
        RoundManager.Instance.OnCharacterCaught(gameObject, wasImposter);
    }
}
