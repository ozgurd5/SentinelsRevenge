using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float movingSpeed = 8f;
    [SerializeField] private float rotatingSpeed = 0.1f;

    private Rigidbody rb;
    private PlayerInputManager pim;
    private PlayerStateData psd;
    private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pim = GetComponent<PlayerInputManager>();
        psd = GetComponent<PlayerStateData>();
        cameraTransform = GameObject.Find("PlayerCamera").transform;
    }

    private void FixedUpdate()
    {
        DecideIdleOrMoving();
        HandleMovement();
    }

    private void DecideIdleOrMoving()
    {
        psd.isMoving = pim.moveInput != Vector2.zero;
        psd.isIdle = !psd.isMoving;
    }

    private void HandleMovement()
    {
        Vector3 movingDirection = cameraTransform.right * pim.moveInput.x + cameraTransform.forward * pim.moveInput.y;
        movingDirection.y = 0f;
        movingDirection *= movingSpeed;

        if (psd.isMoving) transform.forward = Vector3.Slerp(transform.forward, movingDirection, rotatingSpeed);

        rb.velocity = new Vector3(movingDirection.x, rb.velocity.y, movingDirection.z);
    }
}