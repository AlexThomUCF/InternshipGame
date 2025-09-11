using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPCAnimations : MonoBehaviour
{
    public Animator animator;
    public AINavigation ai;

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
}
