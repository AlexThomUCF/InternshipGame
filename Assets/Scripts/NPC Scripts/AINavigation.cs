using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    public NavMeshAgent myAgent;
    public Animator animator;

    public float range = 10f;
    public Transform centrePoint;

    public GameObject[] taskCheckpoints;
    public int choice = 0;

    private NPCDestination currentDestination;
    private GameObject currentTaskTarget;

    public TaskList taskList;

    public bool isPerformingAction = false;
    public bool moving = false;

    private float decisionCooldown = 0f;

    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
        animator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();

        taskCheckpoints = taskList.taskArray;
    }

    void Update()
    {
        if (decisionCooldown > 0)
            decisionCooldown -= Time.deltaTime;

        if (!isPerformingAction &&
            decisionCooldown <= 0 &&
            !myAgent.pathPending &&
            myAgent.remainingDistance <= myAgent.stoppingDistance)
        {
            ChooseAction();
        }

        if (CompareTag("IMPOSTER"))
        {
            taskCheckpoints = taskList.imposterTaskArray;
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

            if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
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
        if (isPerformingAction) return;

        choice = Random.Range(1, 101);

        // 20% Stand Still
        if (choice >= 21 && choice <= 40)
        {
            StartCoroutine(PauseMovement(Random.Range(3f, 5f)));
        }

        // 60% Free Roam
        else if (choice >= 41 && choice <= 100)
        {
            isPerformingAction = true;
            moving = true;

            Vector3 point;

            if (RandomPoint(centrePoint.position, range, out point))
            {
                myAgent.isStopped = false;
                myAgent.SetDestination(point);
                StartCoroutine(ResetAfterMovement(false));
            }
            else
            {
                isPerformingAction = false;
                moving = false;
            }
        }

        // 20% Go To Task
        else
        {
            if (taskCheckpoints.Length == 0) return;

            isPerformingAction = true;
            moving = true;

            int tempNum = Random.Range(0, taskCheckpoints.Length);

            currentTaskTarget = taskCheckpoints[tempNum];

            myAgent.isStopped = false;
            myAgent.SetDestination(currentTaskTarget.transform.position);

            StartCoroutine(ResetAfterMovement(true));
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
        decisionCooldown = 1f;
    }

    // ---------------------------------------------------------
    // MOVEMENT RESET (FOR ROAM + TASK)
    // ---------------------------------------------------------

    IEnumerator ResetAfterMovement(bool isTask)
    {
        while (myAgent.pathPending || myAgent.remainingDistance > myAgent.stoppingDistance)
        {
            yield return null;
        }

        moving = false;

        // Wait at location (makes roaming look natural)
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        if (isTask && currentTaskTarget != null)
        {
            NPCDestination dest = currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null && dest.animationTrigger != "")
            {
                animator.SetTrigger(dest.animationTrigger);
            }

            currentTaskTarget = null;
        }

        isPerformingAction = false;
        decisionCooldown = 1f;
    }

    // ---------------------------------------------------------
    // ANIMATIONS
    // ---------------------------------------------------------

    public void MovementAnimations()
    {
        animator.SetBool("isMoving", moving);
    }
}