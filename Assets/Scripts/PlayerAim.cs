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

    [Header("Detection")]
    public Camera mainCamera;
    public float aimRange = 100f;

    private PlayerControls controls;
    private Outline currentOutline;
    private bool isAiming;

    void Awake()
    {
        controls = new PlayerControls();
        if (crosshair != null)
            crosshair.enabled = false;
        if (mainCamera == null)
            mainCamera = Camera.main;
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

    void Update()
    {
        if (!isAiming) return;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, aimRange))
        {
            var outline = hit.collider.GetComponent<Outline>();
            if (outline != currentOutline)
            {
                ClearOutline();
                if (outline != null)
                {
                    outline.EnableOutline(true);
                    currentOutline = outline;
                }
            }
        }
        else
        {
            ClearOutline();
        }
    }

    void ClearOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.EnableOutline(false);
            currentOutline = null;
        }
    }

    void StartAiming()
    {
        isAiming = true;
        aimCamera.Priority = 20;
        if (crosshair != null) crosshair.enabled = true;
    }

    void StopAiming()
    {
        isAiming = false;
        aimCamera.Priority = 5;
        if (crosshair != null) crosshair.enabled = false;
        ClearOutline();
    }
}
