using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPCAnimations : MonoBehaviour
{
    public Animator animator;
    public AINavigation ai;

    public AnimationClip danceClip;
    public AnimationClip fishingClip;
    public AnimationClip sittingClip;
    public AnimationClip strectchClip;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<AINavigation>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    public void Update()
    {
        MovementAnimations();
    }

    public void MovementAnimations()
    {
        if (ai.moving == true)
        {
            animator.SetBool("isMoving", true);
        }
        else if (ai.moving == false)
        {
            animator.SetBool("isMoving", false);
        }
    }
    public IEnumerator TaskAnimations(NavMeshAgent agent)
    {
        if (this.name == "StageCenter")
        {
            //if agent doesnt have path do this 
            //if it does have a path return
            agent.isStopped = true;
            animator.SetBool("isDancing", true);

            yield return new WaitForSeconds(danceClip.length);

            animator.SetBool("isDancing", false);
            agent.isStopped = false;

        }
        else if (this.name == "PondSpot")
        {
            agent.isStopped = true;
            animator.SetBool("isFishing", true);

            yield return new WaitForSeconds(fishingClip.length);

            animator.SetBool("isFishing", false);
            agent.isStopped = false;
        }
        else if (this.name == "picnic table center")
        {
            agent.isStopped = true;
            animator.SetBool("isSitting", true);

            yield return new WaitForSeconds(sittingClip.length);

            animator.SetBool("isSitting", false);
            agent.isStopped = false;
        }
        else if (this.name == "yoga mats")
        {
            agent.isStopped = true;
            animator.SetBool("isStretching", true);

            yield return new WaitForSeconds(strectchClip.length);

            animator.SetBool("isStretching", false);
            agent.isStopped = false;
        }
        else
        {
            yield return null;
        }
        //paause movement and play clip
    }

}
