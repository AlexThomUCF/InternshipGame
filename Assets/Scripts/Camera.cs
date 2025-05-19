using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    public Vector3 CamOffset = new Vector3(0.6f, 1.5f, -3.5f); // Over-the-shoulder offset
    public float mouseSensitivity = 1f;
    public float controllerSensitivity = 3f;

    private Transform _target;
    private float pitch = 0f;
    private float yaw = 0f;

    private Vector2 lookInput;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Camera.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Camera.canceled += _ => lookInput = Vector2.zero;
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        _target = GameObject.Find("Player").transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        float sensitivity = IsUsingMouse() ? mouseSensitivity : controllerSensitivity;

        // Apply input
        yaw += lookInput.x * sensitivity * Time.deltaTime;
        pitch -= lookInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        // Apply rotation and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = _target.position + rotation * CamOffset;
        transform.LookAt(_target.position + Vector3.up * 1.2f); // Adjust for eye level
    }

    private bool IsUsingMouse()
    {
        // Checks if mouse moved this frame
        return Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;
    }
}
