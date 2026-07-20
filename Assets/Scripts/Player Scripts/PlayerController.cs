
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private Animator anim;
    [SerializeField] private Transform cam;

    [SerializeField] private AudioSource footstepsSound;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip dirtFootsteps;
    [SerializeField] private AudioClip grassFootsteps;
    [SerializeField] private AudioClip woodFootsteps;

    private AudioClip currentFootstepClip;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float turningSpeed = 5f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpHeight = 1f;

    private float verticalVelocity;
    private Vector2 moveInput;
    private bool jumpPressed;

    private PlayerControls controls;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Jump.performed += _ => jumpPressed = true;
        controls.Player.Jump.canceled += _ => jumpPressed = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        Movement();
        HandleFootsteps();

        Vector3 horizontalVel = controller.velocity;
        horizontalVel.y = 0;

        anim.SetFloat("Speed", horizontalVel.magnitude);
    }

    private void Movement()
    {
        GroundMovement();
        Turn();
    }

    private void GroundMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = cam.TransformDirection(move);
        move.y = 0f;

        move *= walkSpeed;
        move.y = VerticalForceCalculation();

        controller.Move(move * Time.deltaTime);
    }

    private void HandleFootsteps()
    {
        UpdateFootstepSound();

        bool isMoving = moveInput != Vector2.zero && controller.isGrounded;

        if (isMoving)
        {
            if (!footstepsSound.isPlaying)
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
        }
    }

    private void UpdateFootstepSound()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.CompareTag("Dirt"))
            {
                currentFootstepClip = dirtFootsteps;
            }
            else if (hit.collider.CompareTag("Grass"))
            {
                currentFootstepClip = grassFootsteps;
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                currentFootstepClip = woodFootsteps;
            }
        }
    }

    private void Turn()
    {
        if (controller.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookDirection = controller.velocity;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turningSpeed);
            }
        }
    }

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;

            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
                jumpPressed = false;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }
}