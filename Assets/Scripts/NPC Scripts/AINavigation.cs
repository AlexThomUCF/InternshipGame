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
        // Freeze AI while being interrogated
        if (isPaused)
        {
            if (myAgent != null)
                myAgent.isStopped = true;

            moving = false;

            if (animator != null)
                animator.SetBool("isMoving", false);

            return;
        }


        // Cooldown before next decision
        if (decisionCooldown > 0)
            decisionCooldown -= Time.deltaTime;


        // Checks when to act
        if (!isPerformingAction &&
            decisionCooldown <= 0 &&
            !myAgent.pathPending &&
            myAgent.remainingDistance <= myAgent.stoppingDistance)
        {
            ChooseAction();
        }


        MovementAnimations();
    }



    // ---------------------------------------------------------
    // RANDOM POINT ON NAVMESH
    // ---------------------------------------------------------

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

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



    // ---------------------------------------------------------
    // ACTION SELECTION
    // ---------------------------------------------------------

    public void ChooseAction()
    {
        if (isPerformingAction)
            return;


        choice = Random.Range(1, 101);


        // Stand still
        if (choice <= 5)
        {
            StartCoroutine(
                PauseMovement(Random.Range(3f, 5f)));
        }


        // Free roam
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
                myAgent.SetDestination(point);

                StartCoroutine(
                    ResetAfterMovement(false));
            }
            else
            {
                isPerformingAction = false;
                moving = false;
            }
        }


        // Task
        else
        {
            if (taskCheckpoints.Length == 0)
                return;


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

            NPCDestination dest = currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null && dest.taskPositions.Length > 0)
            {
                currentTaskPosition = GetAvailableTaskPosition(dest);

                if (currentTaskPosition != null)
                {
                    availableTasks.Remove(currentTaskTarget);

                    myAgent.isStopped = false;
                    myAgent.SetDestination(currentTaskPosition.position);

                    StartCoroutine(ResetAfterMovement(true));
                }
                else
                {
                    // All positions are currently occupied
                    isPerformingAction = false;
                    moving = false;

                    decisionCooldown = 1f;
                }
            }
        }



        // ---------------------------------------------------------
        // STAND STILL
        // ---------------------------------------------------------

        IEnumerator PauseMovement(float pauseTime)
        {
            isPerformingAction = true;
            moving = false;


            myAgent.isStopped = true;


            yield return new WaitForSeconds(pauseTime);


            myAgent.isStopped = false;


            isPerformingAction = false;

            decisionCooldown =
                Random.Range(.5f, 2f);
        }



        // ---------------------------------------------------------
        // MOVEMENT RESET
        // ---------------------------------------------------------

        IEnumerator ResetAfterMovement(bool isTask)
        {
            while (myAgent.pathPending || myAgent.remainingDistance > myAgent.stoppingDistance)
            {
                yield return null;
            }

            moving = false;
            myAgent.isStopped = true;

            // Tell the task that this NPC reached its assigned position
            if (isTask && currentTaskTarget != null)
            {
                TaskManager taskManager =
                    currentTaskTarget.GetComponent<TaskManager>();

                if (taskManager != null)
                {
                    taskManager.NPCReachedTask(this);
                }
            }

            yield return new WaitForSeconds(Random.Range(2f, 4f));

            if (isTask && currentTaskTarget != null)
            {
                NPCDestination dest = currentTaskTarget.GetComponent<NPCDestination>();

                if (dest != null)
                {
                    NPCMemory memory = GetComponent<NPCMemory>();

                    if (memory != null)
                    {
                        memory.AddCompletedTask(dest.taskName);
                        Debug.Log(gameObject.name + " completed task: " + dest.taskName);
                    }

                    if (dest.animationTrigger != "")
                    {
                        myAgent.isStopped = true;
                        moving = false;

                        animator.SetTrigger(dest.animationTrigger);

                        if (animator.isHuman)
                        {
                            Transform attachPoint =
                                animator.GetBoneTransform(dest.attachBone);

                            if (dest.taskObjectPrefab != null && attachPoint != null)
                            {
                                currentTaskObject = Instantiate(
                                    dest.taskObjectPrefab,
                                    attachPoint.position,
                                    dest.taskObjectPrefab.transform.rotation,
                                    attachPoint
                                );

                                currentTaskObject.transform.localPosition = Vector3.zero;
                                currentTaskObject.transform.localRotation = Quaternion.identity;
                            }
                        }

                        // Wait for the task animation to start
                        yield return new WaitUntil(() =>
                            animator.GetCurrentAnimatorStateInfo(0).IsTag("Task")
                        );

                        // Wait for the task animation to finish
                        yield return new WaitUntil(() =>
                            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
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
            decisionCooldown = Random.Range(0.5f, 2f);
        }
    }

    // ---------------------------------------------------------
    // ANIMATIONS
    // ---------------------------------------------------------

    public void MovementAnimations()
    {
        if (animator != null)
            animator.SetBool("isMoving", moving);
    }

    private Transform GetAvailableTaskPosition(NPCDestination dest)
    {
        List<Transform> availablePositions = new List<Transform>();

        foreach (Transform position in dest.taskPositions)
        {
            bool occupied = false;

            Collider[] nearbyNPCs = Physics.OverlapSphere(
                position.position,
                0.75f
            );

            foreach (Collider col in nearbyNPCs)
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
                availablePositions.Add(position);
            }
        }

        if (availablePositions.Count == 0)
            return null;

        return availablePositions[
            Random.Range(0, availablePositions.Count)
        ];
    }
}