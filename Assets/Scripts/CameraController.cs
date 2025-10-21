using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    
    [Space][Header("Move Settings")]
    [SerializeField] private float slowSpeed;
    [SerializeField] private float fastSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    
    [Space][Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;   
    [SerializeField] private float zoomAmount;

    private Vector3 _zoomTarget;
    private Vector3 _newPos;

    private void Awake()
    {
        if (Camera.main != null) 
            cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        _newPos = transform.position;
        _zoomTarget = cameraTransform.localPosition;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    private void HandleMouseInput()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            _zoomTarget += scroll * zoomAmount * cameraTransform.forward;
            
            _zoomTarget.y = Mathf.Clamp(_zoomTarget.y, minZoom, maxZoom);
            _zoomTarget.z = Mathf.Clamp(_zoomTarget.z, minZoom, maxZoom);
        }
        
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            _zoomTarget,
            Time.deltaTime * movementTime
        );
    }

    private void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = slowSpeed;
        }
        
        _newPos += transform.forward * (Input.GetAxis("Vertical") * movementSpeed);
        _newPos += transform.right * (Input.GetAxis("Horizontal") * movementSpeed);

        transform.position = Vector3.Lerp(transform.position, _newPos, Time.deltaTime * movementTime);
    }
}
