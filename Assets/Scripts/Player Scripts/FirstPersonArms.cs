using UnityEngine;

public class FirstPersonArms : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform netGun;

    [Header("Idle")]
    [SerializeField] private float idleSpeed = 1.5f;
    [SerializeField] private float idleAmount = 0.01f;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.025f;

    [Header("Shooting")]
    [SerializeField] private float recoilDistance = 0.12f;
    [SerializeField] private float recoilRotation = 8f;
    [SerializeField] private float recoilReturnSpeed = 10f;

    private Vector3 leftArmStartPosition;
    private Vector3 rightArmStartPosition;

    private Vector3 gunStartPosition;
    private Quaternion gunStartRotation;

    private float bobTimer;
    private float recoilAmount;

    private void Start()
    {
        leftArmStartPosition = leftArm.localPosition;
        rightArmStartPosition = rightArm.localPosition;

        gunStartPosition = netGun.localPosition;
        gunStartRotation = netGun.localRotation;
    }

    private void Update()
    {
        AnimateArms();
        AnimateGun();
    }

    private void AnimateArms()
    {
        bool isWalking =
            playerController != null &&
            playerController.IsMoving;

        if (isWalking)
        {
            bobTimer +=
                Time.deltaTime * walkSpeed;

            float bobX =
                Mathf.Sin(bobTimer) *
                walkBobAmount;

            float bobY =
                Mathf.Abs(
                    Mathf.Cos(bobTimer)
                ) * walkBobAmount;

            leftArm.localPosition =
                leftArmStartPosition +
                new Vector3(
                    bobX,
                    bobY,
                    0f
                );

            rightArm.localPosition =
                rightArmStartPosition +
                new Vector3(
                    bobX,
                    bobY,
                    0f
                );
        }
        else
        {
            bobTimer = 0f;

            float idleY =
                Mathf.Sin(
                    Time.time * idleSpeed
                ) * idleAmount;

            leftArm.localPosition =
                leftArmStartPosition +
                new Vector3(
                    0f,
                    idleY,
                    0f
                );

            rightArm.localPosition =
                rightArmStartPosition +
                new Vector3(
                    0f,
                    idleY,
                    0f
                );
        }
    }

    private void AnimateGun()
    {
        recoilAmount = Mathf.Lerp(
            recoilAmount,
            0f,
            Time.deltaTime *
            recoilReturnSpeed
        );

        Vector3 recoilPosition =
            gunStartPosition +
            new Vector3(
                0f,
                0f,
                -recoilAmount
            );

        Quaternion recoilRotationValue =
            Quaternion.Euler(
                -recoilAmount *
                recoilRotation,
                0f,
                0f
            );

        netGun.localPosition =
            recoilPosition;

        netGun.localRotation =
            gunStartRotation *
            recoilRotationValue;
    }

    public void PlayShootRecoil()
    {
        recoilAmount = recoilDistance;
    }
}
