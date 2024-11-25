using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float cameraFollowSpeed = 2f;
    [SerializeField] private float cameraDistance = 4f;
    [SerializeField] private float lookAngle;
    [SerializeField] private float pivotAngle;
    [SerializeField] private float cameraLookSpeed = 2f;
    [SerializeField] private float cameraPivotSpeed = 2f;
    [SerializeField] private float minPivotAngle = -45f;
    [SerializeField] private float maxPivotAngle = 45f;

    private Vector2 cameraInput;

    private IA_Controls _iaControls;

    private void OnEnable()
    {
        if (_iaControls == null)
        {
            _iaControls = new IA_Controls();
            _iaControls.PlayerControl.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
        }
        _iaControls.Enable();
    }

    private void OnDisable()
    {
        _iaControls.Disable();
    }

    private void RotateCamera()
    {
        lookAngle += cameraInput.x * cameraLookSpeed * sensitivity;
        pivotAngle -= cameraInput.y * cameraPivotSpeed * sensitivity;

        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        Quaternion rotation = Quaternion.Euler(pivotAngle, lookAngle, 0);
        Vector3 direction = rotation * Vector3.back * cameraDistance;
        transform.position = targetTransform.position + direction;
        transform.LookAt(targetTransform);

        Vector3 forwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        targetTransform.rotation = Quaternion.LookRotation(forwardDirection);
    }
    
    private void LateUpdate()
    {
        if(!IsOwner)
            return;
        
        RotateCamera();
    }
}