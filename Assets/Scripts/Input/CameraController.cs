using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float minVerticalAngle = -45f;
    [SerializeField] private float maxVerticalAngle = 45f;
    
    private Vector3 _rotation = Vector3.zero;
    private bool _isRotating = false;

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        
        // Explicitly ignore left mouse button
        if (Input.GetMouseButton(0)) 
        {
            return;
        }
    }

    private void HandleMovement()
    {
        float speed = movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(speed * Time.deltaTime * move);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _isRotating = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            _isRotating = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (_isRotating)
        {
            _rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
            _rotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
            _rotation.y = Mathf.Clamp(_rotation.y, minVerticalAngle, maxVerticalAngle);
            
            transform.eulerAngles = new Vector3(_rotation.y, _rotation.x, 0);
        }
    }
}