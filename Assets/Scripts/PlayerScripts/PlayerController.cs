using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float runningSpeed = 8f;
    [SerializeField] private float jumpSpeed = 10f;
    //[SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float rotatingSpeed = 0.1f;

    private float movingSpeed;

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

        if (psd.isRunning) movingSpeed = runningSpeed;
        else movingSpeed = walkingSpeed;
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
}