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
            xRayAction.action.started += OnXray;
            xRayAction.action.canceled += ctx => xRayScript.StopXray();
        }
    }

    private void OnDisable()
    {
        //Xray will stop when letting go of left click 
        if (xRayAction != null)
        {
            xRayAction.action.started -= OnXray;
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

        if (xRayScript.isRecharging)
            return;

       
        xRayScript.StartXray();

        // Play sound.
        if (audioSource != null &&
            audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

    }
  
}
