using UnityEngine;

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
    [SerializeField] private float gravity = 20f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpCooldown = 1f;

    private float verticalVelocity;
    private float jumpCooldownTimer;

    private Vector2 moveInput;
    private bool jumpPressed;

    private PlayerControls controls;

    // Used by the first-person arms
    public bool IsMoving { get; private set; }

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

        // Tell other scripts whether the player is walking.
        IsMoving =
            moveInput != Vector2.zero &&
            controller.isGrounded;

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

        // Make movement follow the camera/player's
        // horizontal facing direction.
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        move =
            forward * moveInput.y +
            right * moveInput.x;

        move *= walkSpeed;

        move.y = CalculateVerticalMovement();

        controller.Move(
            move * Time.deltaTime
        );
    }

    private float CalculateVerticalMovement()
    {
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (jumpPressed &&
            jumpCooldownTimer <= 0f &&
            controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * gravity * 2f
            );

            jumpCooldownTimer = jumpCooldown;

            PlayJumpSound();

            jumpPressed = false;
        }

        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity -=
            gravity * Time.deltaTime;

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
            Debug.LogWarning(
                "Jump Clip is missing!"
            );
        }
    }

    private void UpdateAnimation()
    {
        Vector3 horizontalVelocity =
            controller.velocity;

        horizontalVelocity.y = 0f;

        if (anim != null)
        {
            anim.SetFloat(
                "Speed",
                horizontalVelocity.magnitude
            );
        }
    }

    private void HandleFootsteps()
    {
        UpdateFootstepSound();

        if (IsMoving &&
            currentFootstepClip != null)
        {
            if (currentFootstepClip !=
                previousFootstepClip)
            {
                if (footstepsSound != null)
                {
                    footstepsSound.Stop();

                    footstepsSound.clip =
                        currentFootstepClip;

                    footstepsSound.Play();
                }

                previousFootstepClip =
                    currentFootstepClip;
            }
            else if (footstepsSound != null &&
                     !footstepsSound.isPlaying)
            {
                footstepsSound.clip =
                    currentFootstepClip;

                footstepsSound.Play();
            }
        }
        else
        {
            if (footstepsSound != null &&
                footstepsSound.isPlaying)
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

        if (footstepsSound != null)
        {
            footstepsSound.volume = volume;
            footstepsSound.pitch = pitch;
        }
    }
}