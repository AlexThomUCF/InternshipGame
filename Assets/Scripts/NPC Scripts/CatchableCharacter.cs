using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CatchableCharacter : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Animation Controllers")]
    public RuntimeAnimatorController parkController;
    public RuntimeAnimatorController arcadeController;
    public RuntimeAnimatorController castleController;

    [Header("Imposter Transformation")]
    [SerializeField] private GameObject tricksterNetPrefab;
    [SerializeField] private GameObject poofVFXPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("Transformation Settings")]
    [SerializeField] private float swapDelay = 0.4f;
    [SerializeField] private float spawnYOffset = 1.5f;

    public bool isCaught { get; private set; }

    public List<Collider> ragDollParts = new List<Collider>();

    private List<Rigidbody> ragdollRigidbodies =
        new List<Rigidbody>();

    private void Awake()
    {
        SetRagdollParts();
        DisableRagdoll();

        string currentSceneName =
            SceneManager.GetActiveScene().name;

        ChooseController(currentSceneName);
    }

    public void Catch(bool wasImposter)
    {
        if (isCaught)
            return;

        isCaught = true;

        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (wasImposter)
        {
            StartCoroutine(TransformIntoTrickster());
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Caught");
            }

            TurnOnRagDoll();
        }

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnCharacterCaught(
                gameObject,
                wasImposter
            );
        }
    }

    private IEnumerator TransformIntoTrickster()
    {
        Vector3 originalPosition = transform.position;

        Vector3 spawnPosition =
            originalPosition + Vector3.up * spawnYOffset;

        Transform target = playerTransform;

        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }

        // Spawn the first poof.
        if (poofVFXPrefab != null)
        {
            Instantiate(
                poofVFXPrefab,
                spawnPosition,
                transform.rotation
            );
        }

        // Wait before swapping the models.
        yield return new WaitForSeconds(swapDelay);

        if (tricksterNetPrefab == null)
        {
            Debug.LogWarning(
                "No Trickster Net Prefab assigned to " +
                gameObject.name
            );

            yield break;
        }

        Quaternion spawnRotation = transform.rotation;

        // Make the trickster face the player.
        if (target != null)
        {
            Vector3 direction =
                target.position - spawnPosition;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Vector3 forward =
                    Vector3.Cross(
                        direction.normalized,
                        Vector3.up
                    );

                spawnRotation =
                    Quaternion.LookRotation(
                        forward,
                        Vector3.up
                    );
            }
        }

        // Spawn the trickster at the adjusted height.
        Instantiate(
            tricksterNetPrefab,
            spawnPosition,
            spawnRotation
        );

        // Remove the original imposter.
        Destroy(gameObject);
    }

    public void TurnOnRagDoll()
    {
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        foreach (Collider c in ragDollParts)
        {
            if (c != null)
            {
                c.isTrigger = false;
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void DisableRagdoll()
    {
        foreach (Collider c in ragDollParts)
        {
            if (c != null)
            {
                c.isTrigger = true;
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    private void SetRagdollParts()
    {
        ragDollParts.Clear();
        ragdollRigidbodies.Clear();

        Collider[] colliders =
            GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                ragDollParts.Add(collider);

                Rigidbody rb = collider.attachedRigidbody;

                if (rb != null &&
                    !ragdollRigidbodies.Contains(rb))
                {
                    ragdollRigidbodies.Add(rb);
                }
            }
        }
    }

    private void ChooseController(string sceneName)
    {
        if (animator == null)
            return;

        if (sceneName == "park_lvl")
        {
            animator.runtimeAnimatorController =
                parkController;
        }
        else if (sceneName == "arcade_lvl")
        {
            animator.runtimeAnimatorController =
                arcadeController;
        }
        else if (sceneName == "castle_lvl")
        {
            animator.runtimeAnimatorController =
                castleController;
        }
        else
        {
            Debug.LogWarning(
                "No animator controller assigned for this scene."
            );
        }
    }
}