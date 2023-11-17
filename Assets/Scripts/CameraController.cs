using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform followTargetTransform;
    [SerializeField] private Vector3 followOffset;

    [Header("Look At")]
    [SerializeField] private Transform lookAtTargetTransform;
    [SerializeField] private PlayerInputManager pim;

    [Header("Limits")]
    [SerializeField] private float cameraMinYDistance = 0.2f;
    [SerializeField] private float cameraMaxYDistance = 7;

    private Vector3 followTargetPreviousPosition;
    private Vector3 followTargetPositionDifference;

    private void Awake()
    {
        //Default values
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
}