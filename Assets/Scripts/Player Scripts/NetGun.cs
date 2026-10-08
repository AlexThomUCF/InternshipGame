using UnityEngine;
using UnityEngine.InputSystem;

public class NetGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject netProjectilePrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [Header("First Person")]
    [SerializeField] private FirstPersonItemSet firstPersonItemSet;

    [Header("Aiming")]
    [SerializeField] private float maxRange = 80f;
    [SerializeField] private LayerMask aimMask = ~0;

    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 35f;

    [Header("Startup")]
    [SerializeField] private float startupCooldown = 2f;

    [Header("Input")]
    [Tooltip("Action bound to <Mouse>/leftButton, <Gamepad>/rightTrigger, etc.")]
    [SerializeField] private InputActionReference fireAction;

    private float startupTimer;

    private Camera Cam =>
        mainCamera != null ? mainCamera : Camera.main;

    private void OnEnable()
    {
        startupTimer = startupCooldown;

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

    private void Update()
    {
        if (startupTimer > 0f)
        {
            startupTimer -= Time.deltaTime;
        }
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (startupTimer > 0f)
            return;

        ShootNet();
    }

    public void ShootNet()
    {
        if (!firePoint || !netProjectilePrefab)
        {
            Debug.LogWarning(
                "NetGun missing FirePoint or Projectile prefab."
            );

            return;
        }

        if (Cam == null)
        {
            Debug.LogWarning(
                "NetGun cannot find the main camera."
            );

            return;
        }

        // Ray from the center of the first-person camera
        Ray ray = Cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 targetPoint;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxRange,
            aimMask,
            QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxRange);
        }

        // Direction from gun to target
        Vector3 dir =
            targetPoint - firePoint.position;

        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = firePoint.forward;
        }

        // Spawn projectile
        GameObject net = Instantiate(
            netProjectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        // Give projectile velocity
        Rigidbody rb =
            net.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity =
                dir.normalized * projectileSpeed;
        }
        else
        {
            Debug.LogWarning(
                "Net projectile is missing a Rigidbody."
            );
        }

        // Play recoil on the NetGun's own item set
        if (firstPersonItemSet != null)
        {
            firstPersonItemSet.PlayShootRecoil();
        }

        // Play sound
        if (audioSource != null &&
            audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}