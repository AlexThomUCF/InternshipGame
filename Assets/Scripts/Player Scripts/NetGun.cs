using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera; // Main Camera
    [SerializeField] private Transform firePoint; // Child of Charles
    [SerializeField] private GameObject netProjectilePrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [Header("Aiming")]
    [SerializeField] private float maxRange = 80f;
    [SerializeField] private LayerMask aimMask = ~0; // Set to Environment + Characters layers; exclude Player

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 35f;

    [Header("Input")]
    [Tooltip("Action bound to <Mouse>/leftButton, <Gamepad>/rightTrigger, etc.")]
    [SerializeField] private InputActionReference fireAction;

    private Camera Cam => mainCamera != null ? mainCamera : Camera.main;

    private void OnEnable()
    {
        if (fireAction != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += OnFire;
        }
    }
    private void OnDisable()
    {
        if (fireAction != null)
        {
            fireAction.action.performed -= OnFire;
            fireAction.action.Disable();
        }
    }

    private void OnFire(InputAction.CallbackContext ctx) => ShootNet();

    public void ShootNet()
    {
        if (!firePoint || !netProjectilePrefab) { Debug.LogWarning("NetGun missing FirePoint or Projectile prefab."); return; }

        // Ray from the camera center (aligned with center of screen)
        Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, aimMask, QueryTriggerInteraction.Ignore))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxRange);

        // Direction from gun to the target point
        Vector3 dir = (targetPoint - firePoint.position);
        if (dir.sqrMagnitude < 0.0001f) dir = firePoint.forward; // safety

        // Spawn and launch projectile
        var net = Instantiate(netProjectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        var rb = net.GetComponent<Rigidbody>();
        rb.velocity = dir.normalized * projectileSpeed;
        audioSource.PlayOneShot(audioClip);
    }
}
