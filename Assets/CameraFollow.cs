using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float distance = 5f;
    public float height = 1.5f;
    public float rotationSpeed = 100f;

    [Header("Camera Collision")]
    public float minDistance = 1f;          // Closest the camera can get
    public float collisionSmooth = 10f;     // How fast it pulls in/out
    public LayerMask collisionMask = ~0;    // What counts as a wall (default: everything)

    private float currentDistance;
    private float currentRotationX;
    private float currentRotationY;

    void Start()
    {
        currentDistance = distance;
        Cursor.lockState = CursorLockMode.Locked;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        currentRotationX += mouseX;
        currentRotationY -= mouseY;
        currentRotationY = Mathf.Clamp(currentRotationY, -30f, 60f);

        // 2. CALCULATE CAMERA POSITION
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 direction = rotation * new Vector3(0, 0, -1);

        // 3. WALL COLLISION (Raycast from player to camera)
        RaycastHit hit;
        float desiredDistance = distance;

        if (Physics.Raycast(targetPosition, direction, out hit, distance, collisionMask))
        {
            // Wall detected - pull camera in
            desiredDistance = Mathf.Max(hit.distance - 0.3f, minDistance);
        }

        // Smoothly move the camera in/out
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, collisionSmooth * Time.deltaTime);

        // 4. APPLY POSITION
        Vector3 cameraOffset = direction * currentDistance;
        transform.position = targetPosition + cameraOffset;
        transform.LookAt(targetPosition);
    }
}