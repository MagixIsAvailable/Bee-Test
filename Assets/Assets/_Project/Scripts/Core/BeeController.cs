using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BeeController : MonoBehaviour
{
     [Header("Flight Settings")]
     public float flySpeed = 15f;
     public float boostMultiplier = 2f;
     public float rotationSpeed = 3f;
     public float hoverStrength = 5f; // Counter-gravity force

     [Header("Camera Settings")]
     public Transform cameraTransform; // Drag your Main Camera here

     private Rigidbody rb;
     private float horizontalInput;
     private float verticalInput;
     private float liftInput;
     private bool isBoosting;

     void Start()
     {
          rb = GetComponent<Rigidbody>();

          // REQUIRED: Standard Rigidbody setup for stable flight
          rb.useGravity = true;
          rb.linearDamping = 2f; // Air resistance (stops bee from sliding forever)
          rb.angularDamping = 5f; // Stops bee from spinning out of control

          // Hide cursor for gameplay
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
     }

     void Update()
     {
          // 1. Get Input
          horizontalInput = Input.GetAxis("Horizontal"); // A/D
          verticalInput = Input.GetAxis("Vertical");     // W/S

          // Ascend (Space) / Descend (Left Shift)
          liftInput = 0f;
          if (Input.GetKey(KeyCode.Space)) liftInput = 1f;
          if (Input.GetKey(KeyCode.LeftShift)) liftInput = -1f;

          isBoosting = Input.GetKey(KeyCode.LeftControl);
     }

     void FixedUpdate()
     {
          HandleMovement();
          HandleRotation();
          ApplyHoverForce();
     }

     void HandleMovement()
     {
          // Calculate direction relative to where the camera is facing
          Vector3 forward = cameraTransform.forward;
          Vector3 right = cameraTransform.right;

          // Flatten direction so we don't fly into the ground when looking down
          forward.y = 0;
          right.y = 0;
          forward.Normalize();
          right.Normalize();

          Vector3 moveDir = (forward * verticalInput) + (right * horizontalInput);

          // Add Up/Down movement
          Vector3 liftDir = Vector3.up * liftInput;

          // Combine inputs
          Vector3 finalForce = moveDir + liftDir;

          // Apply Speed
          float currentSpeed = isBoosting ? flySpeed * boostMultiplier : flySpeed;

          // PHYSX REQUIREMENT: Moving via Physics Forces
          rb.AddForce(finalForce * currentSpeed, ForceMode.Acceleration);
     }

     void HandleRotation()
     {
          // Rotate the Bee to face the same way the Camera is facing
          Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
          rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
     }

     void ApplyHoverForce()
     {
          // Apply a gentle upward force to counter gravity (simulating wings)
          // Only apply if we aren't explicitly flying down
          if (liftInput >= 0)
          {
               rb.AddForce(Vector3.up * hoverStrength, ForceMode.Acceleration);
          }
     }
}