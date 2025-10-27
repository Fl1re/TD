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
    
    private Camera _camera;

    private Bounds _mapBounds;

    private void Awake()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            _camera = Camera.main;
        }
    }

    private void Start()
    {
        _newPos = transform.position;
        _zoomTarget = cameraTransform.localPosition;
        
        InitializeMapBounds();
    }
    
    private void InitializeMapBounds()
    {
        var plane = GameObject.FindWithTag("Map");

        if (plane != null)
        {
            var renderer = plane.GetComponent<Renderer>();
            if (renderer != null)
            {
                _mapBounds = renderer.bounds;
                return;
            }
        }
    }

    private void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }
    
    private void LateUpdate()
    {
        AdjustCameraPositionToBounds();
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
    
    private void AdjustCameraPositionToBounds()
    {
        Plane groundPlane = new Plane(Vector3.up, 0f);

        Vector3[] screenCorners = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(Screen.width, 0, 0),
            new Vector3(0, Screen.height, 0),
            new Vector3(Screen.width, Screen.height, 0)
        };

        float minVisibleX = float.MaxValue, maxVisibleX = float.MinValue;
        float minVisibleZ = float.MaxValue, maxVisibleZ = float.MinValue;

        foreach (var corner in screenCorners)
        {
            Ray ray = _camera.ScreenPointToRay(corner);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                minVisibleX = Mathf.Min(minVisibleX, hitPoint.x);
                maxVisibleX = Mathf.Max(maxVisibleX, hitPoint.x);
                minVisibleZ = Mathf.Min(minVisibleZ, hitPoint.z);
                maxVisibleZ = Mathf.Max(maxVisibleZ, hitPoint.z);
            }
        }

        Vector3 offset = Vector3.zero;

        if (minVisibleX < _mapBounds.min.x) offset.x += _mapBounds.min.x - minVisibleX;
        if (maxVisibleX > _mapBounds.max.x) offset.x += _mapBounds.max.x - maxVisibleX;
        if (minVisibleZ < _mapBounds.min.z) offset.z += _mapBounds.min.z - minVisibleZ;
        if (maxVisibleZ > _mapBounds.max.z) offset.z += _mapBounds.max.z - maxVisibleZ;

        transform.position += offset;
        _newPos += offset;
    }
}
