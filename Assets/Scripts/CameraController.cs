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
    [SerializeField] private float cameraMinYDistance = 3f;
    [SerializeField] private float cameraMaxYDistance = 12f;

    [Header("Fov and Aim")]
    [SerializeField] private float fovChangingSpeed = 100f;
    [SerializeField] private float defaultFovValue = 60f;
    [SerializeField] private float aimFovValue = 40f;
    [SerializeField] private float runningFovValue = 80f;

    private PlayerStateData psd;
    private PlayerInputManager pim;
    private CinemachineVirtualCamera cam;
    private IEnumerator fovChangingRoutine;

    private Vector3 followTargetPreviousPosition;
    private Vector3 followTargetPositionDifference;

    private void Awake()
    {
        psd = GameObject.Find("Player").GetComponent<PlayerStateData>();
        pim = psd.GetComponent<PlayerInputManager>();
        cam = GetComponent<CinemachineVirtualCamera>();

        //Default values
        fovChangingRoutine = ChangeCameraFovRoutine(0);
        transform.position = followTargetTransform.position + followOffset;
        followTargetPreviousPosition = followTargetTransform.position;
    }

    private void LateUpdate()
    {
        if (psd.playerMainState is not (PlayerStateData.PlayerMainState.Normal or PlayerStateData.PlayerMainState.ScriptedEvent)) return;

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

    public enum FovMode
    {
        DefaultFov,
        AimFov,
        RunningFov
    }

    public void ChangeCameraFov(FovMode fovMode)
    {
        StopCoroutine(fovChangingRoutine);
        fovChangingRoutine = ChangeCameraFovRoutine(fovMode);
        StartCoroutine(fovChangingRoutine);
    }

    private IEnumerator ChangeCameraFovRoutine(FovMode fovMode)
    {
        float targetFov;
        if (fovMode == FovMode.DefaultFov) targetFov = defaultFovValue;
        else if (fovMode == FovMode.AimFov) targetFov = aimFovValue;
        else targetFov = runningFovValue;

        if (targetFov > cam.m_Lens.FieldOfView) //If we need to increase
        {
            while (cam.m_Lens.FieldOfView < targetFov)
            {
                cam.m_Lens.FieldOfView += fovChangingSpeed * Time.deltaTime;
                yield return null;
            }
        }

        else
        {
            while (cam.m_Lens.FieldOfView > targetFov)
            {
                cam.m_Lens.FieldOfView -= fovChangingSpeed * Time.deltaTime;
                yield return null;
            }
        }

        cam.m_Lens.FieldOfView = targetFov;
    }
}