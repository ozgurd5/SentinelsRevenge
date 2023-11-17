using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 8f;
    [SerializeField] private float jumpSpeed = 10f;
    //[SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float rotatingSpeed = 0.1f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 5f;

    private float movingSpeed;

    private Rigidbody rb;
    private PlayerInputManager pim;
    private PlayerStateData psd;

    private CameraController cameraController;
    private Transform cameraTransform;

    private bool isIncreasingSpeed;
    private bool isDecreasingSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pim = GetComponent<PlayerInputManager>();
        psd = GetComponent<PlayerStateData>();

        cameraController = GameObject.Find("PlayerCamera").GetComponent<CameraController>();
        cameraTransform = cameraController.transform;

        //Default Value
        movingSpeed = walkingSpeed;
    }

    private void Update()
    {
        DecideIdleOrMoving();
        DecideWalkingOrRunning();

        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void DecideIdleOrMoving()
    {
        psd.isMoving = pim.moveInput != Vector2.zero;
        psd.isIdle = !psd.isMoving;
    }

    private void DecideWalkingOrRunning()
    {
        if (!psd.isMoving) return;

        psd.isRunning = pim.isRunKey;
        psd.isWalking = !psd.isRunning;

        //Walking to running
        if (pim.isRunKeyDown)
        {
            StopSpeedCoroutines();
            StartCoroutine(cameraController.ChangeCameraFov(true));
            StartCoroutine(ChangeSpeed(true, runningSpeed));
        }

        //Running to walking
        else if (pim.isRunKeyUp)
        {
            StopSpeedCoroutines();
            StartCoroutine(cameraController.ChangeCameraFov(false));
            StartCoroutine(ChangeSpeed(false, walkingSpeed));
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
        Vector3 movingDirection = cameraTransform.right * pim.moveInput.x + cameraTransform.forward * pim.moveInput.y;
        movingDirection.y = 0f;
        movingDirection *= movingSpeed;

        if (psd.isMoving) transform.forward = Vector3.Slerp(transform.forward, movingDirection, rotatingSpeed);

        rb.velocity = new Vector3(movingDirection.x, rb.velocity.y, movingDirection.z);
    }

    private void StopSpeedCoroutines()
    {
        StopAllCoroutines();
        isIncreasingSpeed = false;
        isDecreasingSpeed = false;
    }

    private IEnumerator ChangeSpeed(bool isIncreasing, float movingSpeedToReach)
    {
        if (isIncreasing)
        {
            isIncreasingSpeed = true;
            while (movingSpeed < movingSpeedToReach)
            {
                movingSpeed += acceleration * Time.deltaTime;
                yield return null;
            }
            isIncreasingSpeed = false;
        }

        else
        {
            isDecreasingSpeed = true;
            while (movingSpeed > movingSpeedToReach)
            {
                movingSpeed -= acceleration * Time.deltaTime;
                yield return null;
            }
            isDecreasingSpeed = false;
        }

        movingSpeed = movingSpeedToReach;
    }
}