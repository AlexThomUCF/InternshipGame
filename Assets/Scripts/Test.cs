using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour, IInteraction
{
    public UnityEvent onInteract { get; set; } = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        onInteract.Invoke();
        Debug.Log("This is working");
    }
}
