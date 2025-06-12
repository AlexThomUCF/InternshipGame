using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 150f;
    public float JumpForce = 5f;
    public float GravityMultiplier = 2f;

    private Vector2 moveInput;
    private bool jumpPressed = false;

    private bool _isGrounded;
    private Rigidbody _rb;

    public Transform cameraTransform;
    //public Animator animator; FUTURE

    private PlayerControls controls;

    // --- FUTURE Audio ---
    // public AudioSource walkAudioSource;
    // public AudioClip walkClip;
    // public AudioClip jumpClip;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Jump.performed += _ => jumpPressed = true;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned!");
        }

        _rb.freezeRotation = true;

        // FUTURE: setup walking audio
        // if (walkAudioSource != null)
        // {
        //     walkAudioSource.clip = walkClip;
        //     walkAudioSource.loop = true;
        //     walkAudioSource.playOnAwake = false;
        // }
    }

    void Update()
    {
        bool isMoving = moveInput != Vector2.zero;
        //animator.SetBool("isWalking", isMoving); FUTURE

        // FUTURE walking sound logic
        // if (isMoving && _isGrounded && !walkAudioSource.isPlaying)
        //     walkAudioSource.Play();
        // else if (!isMoving && walkAudioSource.isPlaying)
        //     walkAudioSource.Pause();

        if (jumpPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            //animator.SetBool("isJumping", true); FUTURE

            // FUTURE jump sound
            // if (jumpClip != null)
            //     AudioSource.PlayClipAtPoint(jumpClip, transform.position);

            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;

        if (direction != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * RotateSpeed));
        }

        _rb.AddForce(Physics.gravity * GravityMultiplier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            //animator.SetBool("isJumping", false); FUTURE
        }
    }
}
