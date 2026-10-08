using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 30f;
    [SerializeField] private float topClamp = 70f;
    [SerializeField] private float bottomClamp = -40f;

    private float cameraPitch;

    private PlayerControls controls;
    private Vector2 lookInput;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Camera.performed += ctx =>
            lookInput = ctx.ReadValue<Vector2>();

        controls.Player.Camera.canceled += _ =>
            lookInput = Vector2.zero;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Look()
    {
        float mouseX =
            lookInput.x *
            lookSensitivity *
            Time.deltaTime;

        float mouseY =
            lookInput.y *
            lookSensitivity *
            Time.deltaTime;

        // Rotate the player horizontally
        player.Rotate(
            Vector3.up * mouseX
        );

        // Rotate the camera vertically
        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            bottomClamp,
            topClamp
        );

        transform.localRotation =
            Quaternion.Euler(
                cameraPitch,
                0f,
                0f
            );
    }
}