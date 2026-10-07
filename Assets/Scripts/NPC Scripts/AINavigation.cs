using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    [Header("References")]
    private NPCDestination currentDestination;
    private GameObject currentTaskTarget;

    public NavMeshAgent myAgent;
    public Animator animator;
    public Transform centrePoint;
    public GameObject[] taskCheckpoints;
    public TaskList taskList;

    private NPCMemory npcMemory;

    private GameObject currentTaskObject;

    private List<GameObject> availableTasks = new List<GameObject>();

    private Transform currentTaskPosition;

    [Header("Values")]
    public float range = 10f;
    public int choice = 0;
    private float decisionCooldown = 0f;

    [Header("Bools")]
    public bool isPerformingAction = false;
    public bool moving = false;

    [Header("Interrogation")]
    public bool isPaused = false;

    [Header("Blackout Recovery")]
    public float minBlackoutPause = 1f;
    public float maxBlackoutPause = 2f;

    void Start()
    {
        npcMemory = GetComponent<NPCMemory>();

        taskList = FindObjectOfType<TaskList>();

        animator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();

        taskCheckpoints = taskList.taskArray;

        if (CompareTag("IMPOSTER"))
        {
            taskCheckpoints = taskList.imposterTaskArray;
        }
        else
        {
            taskCheckpoints = taskList.taskArray;
        }

        availableTasks.AddRange(taskCheckpoints);
    }

    void Update()
    {
        if (isPaused)
        {
            if (myAgent != null)
                myAgent.isStopped = true;

            moving = false;

            if (animator != null)
                animator.SetBool("isMoving", false);

            return;
        }

        if (decisionCooldown > 0)
        {
            decisionCooldown -= Time.deltaTime;
        }

        if (!isPerformingAction &&
            decisionCooldown <= 0 &&
            !myAgent.pathPending &&
            myAgent.remainingDistance <= myAgent.stoppingDistance)
        {
            ChooseAction();
        }

        MovementAnimations();
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint =
                center + Random.insideUnitSphere * range;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomPoint,
                out hit,
                5.0f,
                NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    public void ChooseAction()
    {
        if (isPerformingAction)
            return;

        if (myAgent == null || !myAgent.isOnNavMesh)
            return;

        choice = Random.Range(1, 101);

        // Random idle/pause
        if (choice <= 5)
        {
            StartCoroutine(
                PauseMovement(Random.Range(3f, 5f))
            );
        }

        // Random walking
        else if (choice <= 10)
        {
            isPerformingAction = true;
            moving = true;

            Vector3 point;

            if (RandomPoint(
                centrePoint.position,
                range,
                out point))
            {
                myAgent.isStopped = false;

                bool success =
                    myAgent.SetDestination(point);

                if (success)
                {
                    StartCoroutine(
                        ResetAfterMovement(false)
                    );
                }
                else
                {
                    isPerformingAction = false;
                    moving = false;

                    myAgent.ResetPath();

                    decisionCooldown = 1f;
                }
            }
            else
            {
                isPerformingAction = false;
                moving = false;

                decisionCooldown = 1f;
            }
        }

        // Task
        else
        {
            if (taskCheckpoints == null ||
                taskCheckpoints.Length == 0)
            {
                return;
            }

            isPerformingAction = true;
            moving = true;

            if (availableTasks.Count == 0)
            {
                availableTasks.AddRange(taskCheckpoints);
            }

            int tempNum =
                Random.Range(0, availableTasks.Count);

            currentTaskTarget =
                availableTasks[tempNum];

            myAgent.isStopped = false;

            NPCDestination dest =
                currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null &&
                dest.taskPositions != null &&
                dest.taskPositions.Length > 0)
            {
                currentTaskPosition =
                    GetAvailableTaskPosition(dest);

                if (currentTaskPosition != null)
                {
                    availableTasks.Remove(
                        currentTaskTarget
                    );

                    myAgent.isStopped = false;

                    bool success =
                        myAgent.SetDestination(
                            currentTaskPosition.position
                        );

                    if (success)
                    {
                        StartCoroutine(
                            ResetAfterMovement(true)
                        );
                    }
                    else
                    {
                        availableTasks.Add(
                            currentTaskTarget
                        );

                        currentTaskTarget = null;
                        currentTaskPosition = null;

                        isPerformingAction = false;
                        moving = false;

                        myAgent.ResetPath();

                        decisionCooldown = 1f;
                    }
                }
                else
                {
                    isPerformingAction = false;
                    moving = false;

                    decisionCooldown = 1f;
                }
            }
            else
            {
                isPerformingAction = false;
                moving = false;

                decisionCooldown = 1f;
            }
        }
    }

    IEnumerator PauseMovement(float pauseTime)
    {
        isPerformingAction = true;
        moving = false;

        myAgent.isStopped = true;

        if (animator != null)
            animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(pauseTime);

        myAgent.isStopped = false;

        isPerformingAction = false;

        decisionCooldown =
            Random.Range(0.5f, 2f);
    }

    IEnumerator ResetAfterMovement(bool isTask)
    {
        // Wait until the NavMeshAgent finishes calculating its path.
        while (myAgent.pathPending)
        {
            yield return null;
        }

        // Make sure the path actually exists.
        if (!myAgent.hasPath ||
            myAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            moving = false;
            isPerformingAction = false;

            myAgent.isStopped = true;
            myAgent.ResetPath();

            decisionCooldown = 1f;

            yield break;
        }

        // If the destination cannot be completely reached,
        // don't allow the NPC to walk forever.
        if (myAgent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            moving = false;
            isPerformingAction = false;

            myAgent.isStopped = true;
            myAgent.ResetPath();

            decisionCooldown = 1f;

            yield break;
        }

        // Wait until the NPC reaches the destination.
        while (myAgent.remainingDistance >
               myAgent.stoppingDistance)
        {
            if (myAgent.pathStatus ==
                NavMeshPathStatus.PathInvalid)
            {
                moving = false;
                isPerformingAction = false;

                myAgent.isStopped = true;
                myAgent.ResetPath();

                decisionCooldown = 1f;

                yield break;
            }

            yield return null;
        }

        moving = false;

        myAgent.isStopped = true;

        if (isTask &&
            currentTaskTarget != null)
        {
            TaskManager taskManager =
                currentTaskTarget.GetComponent<TaskManager>();

            if (taskManager != null)
            {
                taskManager.NPCReachedTask(this);
            }
        }

        yield return new WaitForSeconds(
            Random.Range(2f, 4f)
        );

        if (isTask &&
            currentTaskTarget != null)
        {
            NPCDestination dest =
                currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null)
            {
                NPCMemory memory =
                    GetComponent<NPCMemory>();

                if (memory != null)
                {
                    memory.AddCompletedTask(
                        dest.taskName
                    );

                    Debug.Log(
                        gameObject.name +
                        " completed task: " +
                        dest.taskName
                    );
                }

                if (dest.animationTrigger != "")
                {
                    myAgent.isStopped = true;
                    moving = false;

                    animator.SetTrigger(
                        dest.animationTrigger
                    );

                    if (animator.isHuman)
                    {
                        Transform attachPoint =
                            animator.GetBoneTransform(
                                dest.attachBone
                            );

                        if (dest.taskObjectPrefab != null &&
                            attachPoint != null)
                        {
                            currentTaskObject =
                                Instantiate(
                                    dest.taskObjectPrefab,
                                    attachPoint.position,
                                    dest.taskObjectPrefab.transform.rotation,
                                    attachPoint
                                );

                            currentTaskObject.transform.localPosition =
                                Vector3.zero;

                            currentTaskObject.transform.localRotation =
                                Quaternion.identity;

                            float targetSize = 2.5f;

                            Renderer[] renderers =
                                currentTaskObject
                                .GetComponentsInChildren<Renderer>();

                            if (renderers.Length > 0)
                            {
                                Bounds bounds =
                                    renderers[0].bounds;

                                foreach (
                                    Renderer renderer
                                    in renderers)
                                {
                                    bounds.Encapsulate(
                                        renderer.bounds
                                    );
                                }

                                float currentSize =
                                    Mathf.Max(
                                        bounds.size.x,
                                        bounds.size.y,
                                        bounds.size.z
                                    );

                                if (currentSize > 0.001f)
                                {
                                    float scale =
                                        targetSize /
                                        currentSize;

                                    currentTaskObject
                                        .transform
                                        .localScale =
                                        Vector3.one * scale;
                                }
                            }
                        }

                        yield return new WaitUntil(
                            () =>
                                animator
                                    .GetCurrentAnimatorStateInfo(0)
                                    .IsTag("Task")
                        );

                        yield return new WaitUntil(
                            () =>
                                animator
                                    .GetCurrentAnimatorStateInfo(0)
                                    .normalizedTime >= 1f &&
                                !animator.IsInTransition(0)
                        );

                        if (currentTaskObject != null)
                        {
                            Destroy(currentTaskObject);
                            currentTaskObject = null;
                        }
                    }
                    else
                    {
                        myAgent.isStopped = false;
                        moving = false;
                    }
                }

                currentTaskTarget = null;
                currentTaskPosition = null;
            }

            isPerformingAction = false;

            decisionCooldown =
                Random.Range(0.5f, 2f);
        }
    }

    public void CancelCurrentAction()
    {
        StopAllCoroutines();

        if (currentTaskTarget != null &&
            !availableTasks.Contains(currentTaskTarget))
        {
            availableTasks.Add(
                currentTaskTarget
            );
        }

        if (currentTaskObject != null)
        {
            Destroy(currentTaskObject);
            currentTaskObject = null;
        }

        currentTaskTarget = null;
        currentTaskPosition = null;

        isPerformingAction = false;
        moving = false;

        if (animator != null)
        {
            animator.SetBool(
                "isMoving",
                false
            );
        }

        if (myAgent != null)
        {
            myAgent.isStopped = true;
            myAgent.ResetPath();
        }

        decisionCooldown = 2f;
    }

    public void ResetAfterBlackout()
    {
        if (myAgent == null)
            return;

        if (!myAgent.isOnNavMesh)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                transform.position,
                out hit,
                3f,
                NavMesh.AllAreas))
            {
                myAgent.Warp(hit.position);
            }
        }

        if (!myAgent.isOnNavMesh)
        {
            Debug.LogWarning(
                gameObject.name +
                " could not find the NavMesh after blackout."
            );

            return;
        }

        myAgent.ResetPath();
        myAgent.isStopped = true;

        moving = false;

        // Keep the NPC from immediately choosing
        // another action.
        isPerformingAction = true;

        if (animator != null)
        {
            animator.SetBool(
                "isMoving",
                false
            );
        }

        // Every NPC gets its own random delay.
        decisionCooldown =
            Random.Range(
                minBlackoutPause,
                maxBlackoutPause
            );

        StartCoroutine(
            BlackoutRecovery()
        );
    }

    private IEnumerator BlackoutRecovery()
    {
        float waitTime =
            decisionCooldown;

        yield return new WaitForSeconds(
            waitTime
        );

        if (myAgent == null ||
            !myAgent.isOnNavMesh)
        {
            yield break;
        }

        myAgent.ResetPath();
        myAgent.isStopped = false;

        moving = false;
        isPerformingAction = false;

        decisionCooldown =
            Random.Range(0.5f, 2f);
    }

    public void MovementAnimations()
    {
        if (animator == null ||
            myAgent == null)
            return;

        // Use the actual NavMeshAgent velocity.
        // This prevents the walking animation from
        // playing when the NPC isn't actually moving.
        bool actuallyMoving =
            !myAgent.isStopped &&
            myAgent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool(
            "isMoving",
            actuallyMoving
        );
    }

    private Transform GetAvailableTaskPosition(
        NPCDestination dest
    )
    {
        List<Transform> availablePositions =
            new List<Transform>();

        foreach (
            Transform position
            in dest.taskPositions)
        {
            bool occupied = false;

            Collider[] nearbyNPCs =
                Physics.OverlapSphere(
                    position.position,
                    0.75f
                );

            foreach (
                Collider col
                in nearbyNPCs)
            {
                if (col.gameObject != gameObject &&
                    col.GetComponent<AINavigation>() != null)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
            {
                availablePositions.Add(
                    position
                );
            }
        }

        if (availablePositions.Count == 0)
            return null;

        return availablePositions[
            Random.Range(
                0,
                availablePositions.Count
            )
        ];
    }
}