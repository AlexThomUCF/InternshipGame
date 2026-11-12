using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagnifyingXray : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Xray xRayScript;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;



   [Header("Input")]
    [Tooltip("Action bound to <Mouse>/leftButton, <Gamepad>/rightTrigger, etc.")]
    [SerializeField] private InputActionReference xRayAction;
    // Start is called before the first frame update

    private void OnEnable()
    {
        //Xray will only be active while holding left click
        if(xRayAction != null)
        {
            xRayAction.action.Enable();
            xRayAction.action.started += ctx => xRayScript.StartXray();
            xRayAction.action.canceled += ctx => xRayScript.StopXray();
        }
    }

    private void OnDisable()
    {
        //Xray will stop when letting go of left click 
        if (xRayAction != null)
        {
            xRayAction.action.started -= ctx => xRayScript.StartXray();
            xRayAction.action.canceled -= ctx => xRayScript.StopXray();
            xRayAction.action.Disable();
        }
    }

    private void OnXray(InputAction.CallbackContext ctx)
    {
        // Only allow Xray if the MagnifyingGlass is selected.
        if (!gameObject.activeInHierarchy)
            return;

        // Prevent Xray while paused or on the lose screen.
        if (Time.timeScale == 0f)
            return;

       
        xRayScript.StartXray();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
