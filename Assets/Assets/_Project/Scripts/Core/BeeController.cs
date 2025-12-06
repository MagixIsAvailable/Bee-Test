using UnityEngine;
using UnityEngine.UI; // Required for Slider
public GameOverManager GameOverManager;
[RequireComponent(typeof(Rigidbody))]
public class BeeController : MonoBehaviour
{
     [Header("Flight Settings")]
     public float flySpeed = 15f;
     public float boostMultiplier = 2f;
     public float rotationSpeed = 3f;
     public float hoverStrength = 5f;

     [Header("Stamina Settings")]
     public float maxStamina = 100f;
     public float currentStamina;
     public float staminaDrainRate = 5f;      // How fast energy drops
     public float boostDrainMultiplier = 2f;  // Boosting costs double
     public Slider staminaSlider;             // Drag UI Slider here

     [Header("Camera Settings")]
     public Transform cameraTransform;

     [Header("Audio Settings")]
     public AudioSource flightAudio;
     public float minPitch = 0.8f;
     public float maxPitch = 1.3f;

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
          // Note: linearDamping is for Unity 6. Use 'drag' for older versions.
          rb.linearDamping = 2f;
          rb.angularDamping = 5f;

          baseFlySpeed = flySpeed;
          currentStamina = maxStamina; // Start full

          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          // Bee #2 might be slightly fatter or smaller!
          float randomSize = Random.Range(0.45f, 0.55f);
          transform.localScale = new Vector3(randomSize, randomSize, randomSize);
     }

     void Update()
     {
          // 1. Get Input
          horizontalInput = Input.GetAxis("Horizontal");
          verticalInput = Input.GetAxis("Vertical");

          liftInput = 0f;
          // Only allow flying up if we have Stamina!
          if (Input.GetKey(KeyCode.Space) && currentStamina > 0) liftInput = 1f;
          if (Input.GetKey(KeyCode.LeftShift)) liftInput = -1f;

          isBoosting = Input.GetKey(KeyCode.LeftControl) && currentStamina > 0;

          // 2. Handle Audio Pitch
          HandleAudio();

          // 3. Handle Stamina Drain
          HandleStamina();
     }

     void FixedUpdate()
     {
          HandleMovement();
          HandleRotation();
          ApplyHoverForce();
     }

     void HandleStamina()
     {
          // If moving or boosting, drain stamina
          bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f || liftInput > 0;

          if (isMoving)
          {
               float drain = staminaDrainRate * Time.deltaTime;
               if (isBoosting) drain *= boostDrainMultiplier;

               currentStamina -= drain;
          }

          // Clamp values between 0 and Max
          currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

          // Update UI Slider
          if (staminaSlider != null)
          {
               staminaSlider.value = currentStamina / maxStamina;
          }
     }

     // Called by Flowers to refill energy (Nectar)
     public void RestoreStamina(float amount)
     {
          currentStamina += amount;
          if (currentStamina > maxStamina) currentStamina = maxStamina;

          Debug.Log("Nectar Sipped! Stamina: " + currentStamina);
     }

     void HandleMovement()
     {
          if (cameraTransform == null) return;

          Vector3 forward = cameraTransform.forward;
          Vector3 right = cameraTransform.right;

          forward.y = 0;
          right.y = 0;
          forward.Normalize();
          right.Normalize();

          Vector3 moveDir = (forward * verticalInput) + (right * horizontalInput);
          Vector3 liftDir = Vector3.up * liftInput;
          Vector3 finalForce = moveDir + liftDir;

          // If stamina is 0, disable boost speed
          float currentSpeed = isBoosting ? flySpeed * boostMultiplier : flySpeed;

          rb.AddForce(finalForce * currentSpeed, ForceMode.Acceleration);
     }

     void HandleRotation()
     {
          if (cameraTransform == null) return;

          Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
          rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
     }

     void ApplyHoverForce()
     {
          // Only apply hover force if not diving down
          if (liftInput >= 0)
          {
               rb.AddForce(Vector3.up * hoverStrength, ForceMode.Acceleration);
          }
     }

     void HandleAudio()
     {
          if (flightAudio != null)
          {
               float currentSpeed = rb.linearVelocity.magnitude;
               float speedRatio = Mathf.Clamp01(currentSpeed / (flySpeed * boostMultiplier));
               flightAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
          }
     }

     // Called by GameManager for Pollen Weight
     public void SetEncumbrance(float ratio)
     {
          flySpeed = Mathf.Lerp(baseFlySpeed, baseFlySpeed * 0.4f, ratio);
          rb.mass = 1.0f + (ratio * 2.0f);
     }
}