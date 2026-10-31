using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

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
    [SerializeField] private VisualEffectAsset poofVFXAsset;
    [SerializeField] private Transform playerTransform;

    [Header("Transformation Settings")]
    [SerializeField] private float swapDelay = 0.4f;
    [SerializeField] private float poofLifetime = 3f;
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
            StartCoroutine(
                TransformIntoTrickster()
            );
        }
        else
        {
            // Existing wrong-catch behavior.
            if (animator != null)
            {
                animator.SetTrigger("Caught");
            }

            TurnOnRagDoll();
        }

        // Notify round system.
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
        // Remember original character position.
        Vector3 originalPosition =
            transform.position;

        // Calculate corrected spawn position.
        Vector3 spawnPosition =
            originalPosition +
            Vector3.up * spawnYOffset;

        // Find player/camera.
        Transform target =
            playerTransform;

        if (target == null &&
            Camera.main != null)
        {
            target = Camera.main.transform;
        }

        // Spawn smoke at the original position.
        if (poofVFXAsset != null)
        {
            GameObject poof =
                new GameObject("Capture Poof");

            poof.transform.position =
                originalPosition;

            VisualEffect visualEffect =
                poof.AddComponent<VisualEffect>();

            visualEffect.visualEffectAsset =
                poofVFXAsset;

            visualEffect.Play();

            Destroy(poof, poofLifetime);
        }

        // Wait for smoke to cover the swap.
        yield return new WaitForSeconds(
            swapDelay
        );

        if (tricksterNetPrefab == null)
        {
            Debug.LogWarning(
                "No Trickster Net Prefab assigned to " +
                gameObject.name
            );

            yield break;
        }

        // Calculate rotation so local +X faces player.
        Quaternion spawnRotation =
            transform.rotation;

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

        // Spawn trickster inside net.
        Instantiate(
            tricksterNetPrefab,
            spawnPosition,
            spawnRotation
        );

        // Destroy the original imposter completely.
        Destroy(gameObject);
    }

    public void TurnOnRagDoll()
    {
        // Disable animator and root collider.
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().enabled =
                false;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        // Enable ragdoll physics.
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

                Rigidbody rb =
                    collider.attachedRigidbody;

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
                "No animator controller assigned " +
                "for this scene."
            );
        }
    }
}