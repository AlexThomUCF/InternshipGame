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

    //private Transform rightHand;
    private GameObject currentTaskObject;

    private List<GameObject> availableTasks = new List<GameObject>();

    [Header("Values")]
    public float range = 10f;
    public int choice = 0;
    private float decisionCooldown = 0f;


    [Header("Bools")]
    public bool isPerformingAction = false;
    public bool moving = false;


    void Start()
    {
        taskList = FindObjectOfType<TaskList>();
        animator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();

        taskCheckpoints = taskList.taskArray;


        if (CompareTag("IMPOSTER"))
        {
            taskCheckpoints = taskList.imposterTaskArray;
        }
        else
            taskCheckpoints = taskList.taskArray;

        availableTasks.AddRange(taskCheckpoints);

       // rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
    }

    void Update()
    {
        //Cooldown before next decision
        if (decisionCooldown > 0)  
            decisionCooldown -= Time.deltaTime;  


        //checks when to act
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


    //Decides which action to take IDLE, Free roam, Go to task
    public void ChooseAction()
    {
        if (isPerformingAction) return;

        choice = Random.Range(1, 101);

        // 20% Stand Still (1–20)
        if (choice <= 5)
        {
            StartCoroutine(PauseMovement(Random.Range(3f, 5f)));
        }

        // 60% Free Roam (21–80)
        else if (choice <= 10)
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

        // 20% Go To Task (81–100)
        else
        {
            if (taskCheckpoints.Length == 0) return;

            isPerformingAction = true;
            moving = true;

            // If every task has been completed, refill the list
            if (availableTasks.Count == 0)
            {
                availableTasks.AddRange(taskCheckpoints);
            }

            int tempNum = Random.Range(0, availableTasks.Count);
            currentTaskTarget = availableTasks[tempNum];

            // Remove it so it can't be picked again
            availableTasks.RemoveAt(tempNum);

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
        decisionCooldown = Random.Range(0.5f, 2f);
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

        // Wait at location 
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        if (isTask && currentTaskTarget != null)
        {
            NPCDestination dest = currentTaskTarget.GetComponent<NPCDestination>();

            if (dest != null && dest.animationTrigger != "")
            {
                animator.SetTrigger(dest.animationTrigger); //Plays animation at task location

                if (animator.isHuman)
                {
                    Transform attachPoint = animator.GetBoneTransform(dest.attachBone);

                    if (dest.taskObjectPrefab != null && attachPoint != null)
                    {
                        currentTaskObject = Instantiate(
                            dest.taskObjectPrefab,
                            attachPoint.position,
                            attachPoint.rotation,
                            attachPoint
                        );

                        currentTaskObject.transform.localPosition = Vector3.zero;
                        currentTaskObject.transform.localRotation = Quaternion.identity;
                    }
                }

                // Wait until animation starts
                yield return null;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Wait for animation to finish
                yield return new WaitForSeconds(stateInfo.length);

                if (currentTaskObject != null)
                {
                    Destroy(currentTaskObject);
                    currentTaskObject = null;
                }
            } 

            currentTaskTarget = null;
        }

        isPerformingAction = false;
        decisionCooldown = Random.Range(0.5f, 2f);
    }

    // ---------------------------------------------------------
    // ANIMATIONS
    // ---------------------------------------------------------

    public void MovementAnimations()
    {
        animator.SetBool("isMoving", moving);
    }

    /*Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);

            if (result != null)
                return result;
        }

        return null;
    }*/

}