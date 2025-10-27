using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatchableCharacter : MonoBehaviour
{
    public NavMeshAgent agent;    // Assign NavMeshAgent
    public Animator animator;     // For possible future "Caught" animation
    public bool isCaught { get; private set; }
    public List<Collider> ragDollParts = new List<Collider>();

    private void Awake()
    {
        SetRagdollParts();
    }
    public void Catch(bool wasImposter)
    {
        if (isCaught) return;
        isCaught = true;

        if (agent) agent.isStopped = true;
        if (animator) animator.SetTrigger("Caught"); // Create a "Caught" trigger

        // Notify round system
        RoundManager.Instance.OnCharacterCaught(gameObject, wasImposter);
    }

    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                collider.isTrigger = true;
                ragDollParts.Add(collider);
            }

        }
    }

}
