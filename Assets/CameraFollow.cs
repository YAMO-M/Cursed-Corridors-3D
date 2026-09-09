using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;          
    public float distance = 5f;       // How far behind the player
    public float height = 2f;         // How high above the player
    public float rotationSpeed = 100f;

    private float currentRotationX;
    private float currentRotationY;

    void Start()
    {
        // Lock the mouse cursor so it doesn't leave the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        currentRotationX += mouseX;
        currentRotationY -= mouseY;
        currentRotationY = Mathf.Clamp(currentRotationY, -30f, 60f); // Limits up/down look

        // 2. CALCULATE POSITION 
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 cameraOffset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = targetPosition + cameraOffset;

        // 3. APPLY POSITION 
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
        transform.LookAt(targetPosition);
    }
}