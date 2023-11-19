using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 8f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float rotatingSpeed = 0.1f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 10f;

    private Vector3 movingDirection;
    private float movingSpeed;

    private Rigidbody rb;
    private PlayerStateData psd;
    private PlayerInputManager pim;

    private CameraController cameraController;
    private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        psd = GetComponent<PlayerStateData>();
        pim = GetComponent<PlayerInputManager>();

        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();
        cameraTransform = cameraController.transform;

        //Default Value
        movingSpeed = walkingSpeed;
    }

    private void Update()
    {
        if (psd.playerMainState != PlayerStateData.PlayerMainState.Normal) return;

        DecideIdleOrMoving();
        DecideWalkingOrRunning();

        HandleRotation();
        HandleJump();
    }

    private void FixedUpdate()
    {
        if (psd.playerMainState != PlayerStateData.PlayerMainState.Normal) return;

        HandleMovement();
    }

    private void DecideIdleOrMoving()
    {
        psd.isMoving = pim.moveInput != Vector2.zero;
        psd.isIdle = !psd.isMoving;
    }

    private void DecideWalkingOrRunning()
    {
        //TODO FIX: PRESSING RUN KEY BEFORE MOVING DON'T MAKE PLAYER RUN

        if (!psd.isMoving) return;

        psd.isRunning = pim.isRunKey;
        psd.isWalking = !psd.isRunning;

        //Walking to running
        if (pim.isRunKeyDown && !psd.isAiming)
        {
            StopSpeedCoroutines();
            StartCoroutine(ChangeSpeed(true, runningSpeed));
            cameraController.ChangeCameraFov(CameraController.FovMode.RunningFov);
        }

        //Running to walking
        else if (pim.isRunKeyUp && !psd.isAiming)
        {
            StopSpeedCoroutines();
            StartCoroutine(ChangeSpeed(false, walkingSpeed));
            cameraController.ChangeCameraFov(CameraController.FovMode.DefaultFov);
        }
    }

    private void HandleJump()
    {
        if (psd.isGrounded && pim.isJumpKeyDown)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            psd.isJumping = true;
        }
    }

    private void HandleMovement()
    {
        movingDirection = cameraTransform.right * pim.moveInput.x + cameraTransform.forward * pim.moveInput.y;
        movingDirection.y = 0f;
        movingDirection *= movingSpeed;

        rb.velocity = new Vector3(movingDirection.x, rb.velocity.y, movingDirection.z);
    }

    private void HandleRotation()
    {
        if (psd.isRangedAttacking) return;

        if (psd.isAiming || psd.isMeleeAttacking)
        {
            transform.forward = Vector3.Slerp(transform.forward, cameraTransform.forward, rotatingSpeed * 2);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        else if (psd.isMoving) transform.forward = Vector3.Slerp(transform.forward, movingDirection, rotatingSpeed);
    }

    private void StopSpeedCoroutines()
    {
        StopAllCoroutines();
    }

    private IEnumerator ChangeSpeed(bool isIncreasing, float movingSpeedToReach)
    {
        if (isIncreasing)
        {
            while (movingSpeed < movingSpeedToReach)
            {
                movingSpeed += acceleration * Time.deltaTime;
                yield return null;
            }
        }

        else
        {
            while (movingSpeed > movingSpeedToReach)
            {
                movingSpeed -= deceleration * Time.deltaTime;
                yield return null;
            }
        }

        movingSpeed = movingSpeedToReach;
    }
}