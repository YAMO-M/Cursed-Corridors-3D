using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float distance = 5f;
    public float height = 1.5f;
    public float rotationSpeed = 100f;
    public float fixedPitch = 20f; 

    [Header("Camera Collision")]
    public float minDistance = 1f;
    public float collisionSmooth = 10f;
    public LayerMask collisionMask = ~0;

    private float currentDistance;
    private float currentRotationX;

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

        // 1. MOUSE LOOK — horizontal only
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        currentRotationX += mouseX;

        // 2. CALCULATE CAMERA POSITION — pitch is now fixed, not mouse-driven
        Quaternion rotation = Quaternion.Euler(fixedPitch, currentRotationX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 direction = rotation * new Vector3(0, 0, -1);

        // 3. WALL COLLISION
        RaycastHit hit;
        float desiredDistance = distance;

        if (Physics.Raycast(targetPosition, direction, out hit, distance, collisionMask))
        {
            desiredDistance = Mathf.Max(hit.distance - 0.3f, minDistance);
        }

        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, collisionSmooth * Time.deltaTime);

        // 4. APPLY POSITION
        Vector3 cameraOffset = direction * currentDistance;
        transform.position = targetPosition + cameraOffset;
        transform.LookAt(targetPosition);
    }
}