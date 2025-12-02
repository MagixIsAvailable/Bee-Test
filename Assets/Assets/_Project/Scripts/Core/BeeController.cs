using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BeeController : MonoBehaviour
{
     [Header("Flight Settings")]
     public float flySpeed = 15f;
     public float boostMultiplier = 2f;
     public float rotationSpeed = 3f;
     public float hoverStrength = 5f;

     [Header("Camera Settings")]
     public Transform cameraTransform;

     [Header("Audio Settings")]
     public AudioSource flightAudio; // Drag the AudioSource here!
     public float minPitch = 0.8f;   // Idle pitch
     public float maxPitch = 1.3f;   // Full speed pitch

     private Rigidbody rb;
     private float horizontalInput;
     private float verticalInput;
     private float liftInput;
     private bool isBoosting;
     private float baseFlySpeed;

     void Start()
     {
          rb = GetComponent<Rigidbody>();
          rb.useGravity = true;
          rb.linearDamping = 2f;
          rb.angularDamping = 5f;

          baseFlySpeed = flySpeed;
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
     }

     void Update()
     {
          // Inputs
          horizontalInput = Input.GetAxis("Horizontal");
          verticalInput = Input.GetAxis("Vertical");

          liftInput = 0f;
          if (Input.GetKey(KeyCode.Space)) liftInput = 1f;
          if (Input.GetKey(KeyCode.LeftShift)) liftInput = -1f;

          isBoosting = Input.GetKey(KeyCode.LeftControl);

          // Update Audio Pitch
          HandleAudio();
     }

     void FixedUpdate()
     {
          HandleMovement();
          HandleRotation();
          ApplyHoverForce();
     }

     void HandleMovement()
     {
          Vector3 forward = cameraTransform.forward;
          Vector3 right = cameraTransform.right;

          forward.y = 0;
          right.y = 0;
          forward.Normalize();
          right.Normalize();

          Vector3 moveDir = (forward * verticalInput) + (right * horizontalInput);
          Vector3 liftDir = Vector3.up * liftInput;
          Vector3 finalForce = moveDir + liftDir;

          float currentSpeed = isBoosting ? flySpeed * boostMultiplier : flySpeed;

          rb.AddForce(finalForce * currentSpeed, ForceMode.Acceleration);
     }

     void HandleRotation()
     {
          Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
          rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
     }

     void ApplyHoverForce()
     {
          if (liftInput >= 0)
          {
               rb.AddForce(Vector3.up * hoverStrength, ForceMode.Acceleration);
          }
     }

     void HandleAudio()
     {
          if (flightAudio != null)
          {
               // Calculate speed (0 to 1 ratio)
               // Note: In Unity 6, rb.velocity might be renamed to rb.linearVelocity. 
               // If this line turns red, change 'velocity' to 'linearVelocity'
               float currentSpeed = rb.linearVelocity.magnitude;

               float speedRatio = Mathf.Clamp01(currentSpeed / (flySpeed * boostMultiplier));

               // Lerp the pitch: Low when slow, High when fast
               flightAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
          }
     }

     // Called by Manager for Pollen Weight
     public void SetEncumbrance(float ratio)
     {
          flySpeed = Mathf.Lerp(baseFlySpeed, baseFlySpeed * 0.4f, ratio);
          rb.mass = 1.0f + (ratio * 2.0f);
     }
}