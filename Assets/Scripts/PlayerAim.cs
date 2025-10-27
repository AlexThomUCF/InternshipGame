using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera aimCamera;

    [Header("UI")]
    public Image crosshair;

    private PlayerControls controls;
    private bool isAiming;

    void Awake()
    {
        controls = new PlayerControls();

        // Hide crosshair at game start
        if (crosshair != null)
            crosshair.enabled = false;
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Aim.started += ctx => StartAiming();
        controls.Player.Aim.canceled += ctx => StopAiming();
    }

    void OnDisable()
    {
        controls.Player.Aim.started -= ctx => StartAiming();
        controls.Player.Aim.canceled -= ctx => StopAiming();
        controls.Disable();
    }

    void StartAiming()
    {
        isAiming = true;
        aimCamera.Priority = 20;

        if (crosshair != null)
            crosshair.enabled = true;
    }

    void StopAiming()
    {
        isAiming = false;
        aimCamera.Priority = 5;

        if (crosshair != null)
            crosshair.enabled = false;
    }
}