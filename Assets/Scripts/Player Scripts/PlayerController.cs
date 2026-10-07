using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cam;

    private CharacterController controller;
    private Animator anim;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepsSound;
    [SerializeField] private AudioClip jumpClip;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip dirtFootsteps;
    [SerializeField] private AudioClip grassFootsteps;
    [SerializeField] private AudioClip woodFootsteps;

    private AudioClip currentFootstepClip;
    private AudioClip previousFootstepClip;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float turningSpeed = 5f;
    [SerializeField] private float gravity = 20f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpCooldown = 1f;

    private float verticalVelocity;
    private float jumpCooldownTimer;

    private Vector2 moveInput;
    private bool jumpPressed;

    private PlayerControls controls;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx =>
            moveInput = ctx.ReadValue<Vector2>();

        controls.Player.Move.canceled += _ =>
            moveInput = Vector2.zero;

        controls.Player.Jump.performed += _ =>
        {
            // Only accept the jump if the cooldown is finished
            if (jumpCooldownTimer <= 0f)
            {
                jumpPressed = true;
            }
        };
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        Move();
        Turn();
        HandleFootsteps();
        UpdateAnimation();
    }

    private void Move()
    {
        Vector3 move = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        move = cam.TransformDirection(move);
        move.y = 0f;

        move *= walkSpeed;
        move.y = CalculateVerticalMovement();

        controller.Move(move * Time.deltaTime);
    }

    private float CalculateVerticalMovement()
    {
        // Reduce jump cooldown
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        // Jump
        if (jumpPressed && jumpCooldownTimer <= 0f)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * gravity * 2f
            );

            // Start cooldown
            jumpCooldownTimer = jumpCooldown;

            PlayJumpSound();

            // Consume the jump press
            jumpPressed = false;
        }

        // Apply gravity
        verticalVelocity -= gravity * Time.deltaTime;

        return verticalVelocity;
    }

    private void PlayJumpSound()
    {
        if (jumpClip != null)
        {
            AudioSource.PlayClipAtPoint(
                jumpClip,
                transform.position,
                0.25f
            );
        }
        else
        {
            Debug.LogWarning("Jump Clip is missing!");
        }
    }

    private void Turn()
    {
        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(velocity);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * turningSpeed
            );
        }
    }

    private void UpdateAnimation()
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        anim.SetFloat(
            "Speed",
            horizontalVelocity.magnitude
        );
    }

    private void HandleFootsteps()
    {
        UpdateFootstepSound();

        bool isMoving =
            moveInput != Vector2.zero &&
            controller.isGrounded;

        if (isMoving && currentFootstepClip != null)
        {
            // Surface changed
            if (currentFootstepClip != previousFootstepClip)
            {
                footstepsSound.Stop();

                footstepsSound.clip = currentFootstepClip;
                footstepsSound.Play();

                previousFootstepClip = currentFootstepClip;
            }
            // Footstep finished, play it again
            else if (!footstepsSound.isPlaying)
            {
                footstepsSound.clip = currentFootstepClip;
                footstepsSound.Play();
            }
        }
        else
        {
            if (footstepsSound.isPlaying)
            {
                footstepsSound.Stop();
            }

            previousFootstepClip = null;
        }
    }

    private void UpdateFootstepSound()
    {
        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            2f))
        {
            if (hit.collider.CompareTag("Dirt"))
            {
                SetFootstep(
                    dirtFootsteps,
                    1f,
                    1f
                );
            }
            else if (hit.collider.CompareTag("Grass"))
            {
                SetFootstep(
                    grassFootsteps,
                    1f,
                    1f
                );
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                SetFootstep(
                    woodFootsteps,
                    1.5f,
                    1.5f
                );
            }
            else
            {
                currentFootstepClip = null;
            }
        }
        else
        {
            currentFootstepClip = null;
        }
    }

    private void SetFootstep(
        AudioClip clip,
        float volume,
        float pitch)
    {
        currentFootstepClip = clip;

        footstepsSound.volume = volume;
        footstepsSound.pitch = pitch;
    }
}