using UnityEngine;

public class FirstPersonItemSet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Arms")]
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    [Header("Idle")]
    [SerializeField] private float idleSpeed = 1.5f;
    [SerializeField] private float idleAmount = 0.01f;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.025f;

    [Header("Recoil")]
    [SerializeField] private Transform recoilObject;
    [SerializeField] private float recoilDistance = 0.12f;
    [SerializeField] private float recoilRotation = 8f;
    [SerializeField] private float recoilReturnSpeed = 10f;

    private Vector3 leftArmStartPosition;
    private Vector3 rightArmStartPosition;

    private Vector3 recoilStartPosition;
    private Quaternion recoilStartRotation;

    private float bobTimer;
    private float recoilAmount;

    private void Start()
    {
        if (leftArm != null)
        {
            leftArmStartPosition =
                leftArm.localPosition;
        }

        if (rightArm != null)
        {
            rightArmStartPosition =
                rightArm.localPosition;
        }

        if (recoilObject != null)
        {
            recoilStartPosition =
                recoilObject.localPosition;

            recoilStartRotation =
                recoilObject.localRotation;
        }
    }

    private void Update()
    {
        bool isMoving =
            playerController != null &&
            playerController.IsMoving;

        AnimateArms(isMoving);
        AnimateRecoil();
    }

    private void AnimateArms(bool isMoving)
    {
        if (leftArm == null ||
            rightArm == null)
        {
            return;
        }

        if (isMoving)
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

    private void AnimateRecoil()
    {
        if (recoilObject == null)
            return;

        recoilAmount = Mathf.Lerp(
            recoilAmount,
            0f,
            Time.deltaTime *
            recoilReturnSpeed
        );

        recoilObject.localPosition =
            recoilStartPosition +
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

        recoilObject.localRotation =
            recoilStartRotation *
            recoilRotationValue;
    }

    public void PlayShootRecoil()
    {
        recoilAmount = recoilDistance;
    }
}