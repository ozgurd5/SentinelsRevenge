using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Header("Follow and Look At")]
    [SerializeField] private Transform lookAtTargetTransform;
    [SerializeField] private Transform followTargetTransform;
    [SerializeField] private Vector3 followOffset;

    [Header("Control")]
    [SerializeField] private PlayerInputManager pim;
    [SerializeField] private float cameraMinYDistance = 0.2f;
    [SerializeField] private float cameraMaxYDistance = 7;

    [Header("FOV Change Settings")]
    [SerializeField] private float defaultFov;
    [SerializeField] private float fovIncreaseAmount = 20f;
    [SerializeField] private float fovChangeTime = 0.1f;

    [SerializeField] private CinemachineVirtualCamera cam;
    private bool isFovIncreased;

    private Vector3 followTargetPreviousPosition;
    private Vector3 followTargetPositionDifference;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();

        //Default values
        defaultFov = cam.m_Lens.FieldOfView;
        transform.position = followTargetTransform.position + followOffset;
        followTargetPreviousPosition = followTargetTransform.position;
    }

    private void LateUpdate()
    {
        followTargetPositionDifference = followTargetTransform.position - followTargetPreviousPosition;
        transform.position += followTargetPositionDifference;
        followTargetPreviousPosition = followTargetTransform.position;

        ControlCamera();
    }

    private void ControlCamera()
    {
        transform.RotateAround(lookAtTargetTransform.position, Vector3.up, pim.lookInput.x);
        transform.RotateAround(lookAtTargetTransform.position, transform.right, pim.lookInput.y);

        float downLimit = lookAtTargetTransform.position.y - cameraMinYDistance;
        float upLimit = lookAtTargetTransform.position.y + cameraMaxYDistance;

        if ((transform.position.y < downLimit && pim.lookInput.y < 0) || (transform.position.y > upLimit && pim.lookInput.y > 0))
        {
            transform.RotateAround(lookAtTargetTransform.position, transform.right, -pim.lookInput.y);
        }

        transform.LookAt(lookAtTargetTransform);
    }

    //TODO: BETTER
    public IEnumerator ChangeCameraFov(bool isIncreasing)
    {
        float timePassed = 0f;
        float speed = fovIncreaseAmount / fovChangeTime;
        
        while (timePassed <= fovChangeTime)
        {
            if (isIncreasing) cam.m_Lens.FieldOfView += speed * Time.deltaTime;
            else cam.m_Lens.FieldOfView -= speed * Time.deltaTime;

            if (isIncreasing && (cam.m_Lens.FieldOfView >= defaultFov + fovIncreaseAmount)) break;
            if (!isIncreasing && (cam.m_Lens.FieldOfView <= defaultFov)) break;

            timePassed += Time.deltaTime;
            yield return null;
        }

        if (isIncreasing) cam.m_Lens.FieldOfView = defaultFov + fovIncreaseAmount;
        else cam.m_Lens.FieldOfView = defaultFov;

        isFovIncreased = isIncreasing;
    }
}