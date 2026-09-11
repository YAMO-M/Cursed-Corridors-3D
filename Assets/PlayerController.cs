using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 2.5f;

    [Header("Smoothness")]
    public float acceleration = 10f;
    public float deceleration = 8f;

    [Header("Camera")]
    public Transform cameraTransform;  // Drag your Main Camera here

    private Rigidbody rb;
    private Animator animator;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // If no camera is set, find the main camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. GET INPUT
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 2. CALCULATE MOVEMENT RELATIVE TO CAMERA
        if (inputDirection.magnitude >= 0.1f)
        {
            // Get the camera's forward and right directions (ignore pitch)
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0;
            right.Normalize();

            // Calculate movement direction relative to camera
            Vector3 moveDirection = forward * inputDirection.z + right * inputDirection.x;
            moveDirection.Normalize();

            // Rotate player to face movement direction
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            // Check if sprinting
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // Accelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            rb.linearVelocity = moveDirection * currentSpeed;
        }
        else
        {
            // Decelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            rb.linearVelocity = Vector3.zero;
        }

        // 3. ANIMATOR
        if (animator != null)
        {
            float speedPercent = currentSpeed / walkSpeed;
            animator.SetFloat("Speed", speedPercent);
        }
    }
}