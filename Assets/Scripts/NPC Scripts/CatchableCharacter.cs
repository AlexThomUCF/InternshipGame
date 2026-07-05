using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CatchableCharacter : MonoBehaviour
{
    public NavMeshAgent agent;    // Assign NavMeshAgent

    [Header("Animaton Controllers")]
    public Animator animator;     // For possible future "Caught" animation
    public RuntimeAnimatorController parkController;
    public RuntimeAnimatorController arcadeController;
    public RuntimeAnimatorController castleController;

    public bool isCaught { get; private set; }
    public List<Collider> ragDollParts = new List<Collider>();

    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();

    private void Awake()
    {
        SetRagdollParts();
        DisableRagdoll();
        string currentSceneName = SceneManager.GetActiveScene().name;
        ChooseController(currentSceneName);
    }

    public void Catch(bool wasImposter)
    {
        if (isCaught) return;
        isCaught = true;

        if (agent) agent.isStopped = true;
        if (animator) animator.SetTrigger("Caught"); // Create a "Caught" trigger
        
        TurnOnRagDoll();

        // Notify round system
        RoundManager.Instance.OnCharacterCaught(gameObject, wasImposter);
    }

    public void TurnOnRagDoll()
    {
        // Disable animator and root collider
        if (GetComponent<BoxCollider>())
            GetComponent<BoxCollider>().enabled = false;

        animator.enabled = false;

        // Enable physics for all ragdoll parts
        foreach (Collider c in ragDollParts)
        {
            c.isTrigger = false;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void DisableRagdoll()
    {
        // Disable physics while animating
        foreach (Collider c in ragDollParts)
        {
            c.isTrigger = true;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }
    private void SetRagdollParts()
    {
        ragDollParts.Clear();
        ragdollRigidbodies.Clear();

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                ragDollParts.Add(collider);

                Rigidbody rb = collider.attachedRigidbody;
                if (rb != null)
                    ragdollRigidbodies.Add(rb);
            }
        }
    }

    void ChooseController(string sceneName)
    {
        // Check if the scene name matches
        if (sceneName == "park_lvl")
        {
            animator.runtimeAnimatorController = parkController;
        }
        else if (sceneName == "arcade_lvl")
        {
            animator.runtimeAnimatorController = arcadeController;
        }
        else if(sceneName == "castle_lvl")
        {
            animator.runtimeAnimatorController = castleController;
        }
        // Add more conditions if you have more scenes and controllers
        else
        {
            Debug.LogWarning("No animator controller assigned for this scene.");
        }
    }

}
