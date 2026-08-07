using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeeballMachine : MonoBehaviour
{
    SkeeballAnimations skee;
    // Start is called before the first frame update
    void Start()
    {
        skee = FindObjectOfType<SkeeballAnimations>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playAnim()
    {
        skee.playTheClip();
    }
}
