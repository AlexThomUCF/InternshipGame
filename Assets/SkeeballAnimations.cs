using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeeballAnimations : MonoBehaviour
{
    public Animator animator;

    public GameObject skeeBall;
    public GameObject particle;
    public Transform particlePosition;
    
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBall()
    {
        GameObject ball = Instantiate(skeeBall);
    }

    public void BallActive()
    {
        skeeBall.SetActive(true);
    }
    public void BallInActive()
    {
        skeeBall.SetActive(false);
    }
    public void SpawnParticle()
    {
        GameObject confetti = Instantiate(particle, particlePosition);
    }
    public void playTheClip()
    {
        animator.SetTrigger("SkeeMachine");
    }
}
