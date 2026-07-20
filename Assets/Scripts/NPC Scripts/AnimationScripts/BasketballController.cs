using UnityEngine;

public class BasketballController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartDribbling()
    {
        animator.SetBool("IsDribbling", true);
    }

    public void StopDribbling()
    {
        animator.SetBool("IsDribbling", false);
    }
}