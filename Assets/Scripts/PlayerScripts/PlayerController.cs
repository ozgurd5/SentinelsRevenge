using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float cameraMinYPos = 0.2f;
    [SerializeField] private float cameraMaxYPos = 7;

    private Rigidbody rb;
    private PlayerInputManager pim;

    private Transform cameraFollowTransform;
    private Transform cameraLookAtTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pim = GetComponent<PlayerInputManager>();

        cameraFollowTransform = transform.GetChild(2);
        cameraLookAtTransform = transform.GetChild(3);
    }

    private void Update()
    {
        HandleLooking();
    }

    private void FixedUpdate()
    {
        Vector2 moving = pim.moveInput * speed;
        rb.velocity = new Vector3(moving.x, rb.velocity.y, moving.y);
    }

    private void HandleLooking()
    {
        cameraFollowTransform.RotateAround(cameraLookAtTransform.position, Vector3.up, pim.lookInput.x);
        cameraFollowTransform.RotateAround(cameraLookAtTransform.position, cameraFollowTransform.right, pim.lookInput.y);

        if ((cameraFollowTransform.position.y < cameraMinYPos && pim.lookInput.y < 0) || (cameraFollowTransform.position.y > cameraMaxYPos && pim.lookInput.y > 0))
        {
            cameraFollowTransform.RotateAround(cameraLookAtTransform.position, cameraFollowTransform.right, -pim.lookInput.y);
        }
    }
}