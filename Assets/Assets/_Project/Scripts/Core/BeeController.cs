/*
This script acts as the central character controller, utilizing Unity’s Rigidbody physics engine to simulate realistic insect flight. It processes user input (WASD, Space, Shift) to apply forces for movement, lift, and rotation, while calculating drag and angular damping to create air resistance rather than instant arcade movement. 

Key features include:
1. Physics-Based Flight: Manages momentum, drag, and hover mechanics.
2. Vitality System: Drains stamina based on movement intensity (boost vs. normal) and handles exhaustion logic.
3. Animation Control: Communicates with the Animator component to toggle the "isFlying" state (TakeOff/Hover/Land) based on the bee's life status.
4. Game Over Trigger: Detects when the player has run out of energy and crashed (velocity near zero), effectively disabling controls and triggering the Game Over UI.
5. Audio & UI: Modulates engine pitch based on speed and updates the Stamina Slider in real-time.
*/



using UnityEngine;     // Required for Unity components
using UnityEngine.UI; // Required for Slider

[RequireComponent(typeof(Rigidbody))]
public class BeeController : MonoBehaviour
{
     [Header("Game Manager Connection")]
     public GameOverManager gameOverManager; // Reference to GameOverManager

     [Header("Animation Settings")]
     public Animator beeAnimator; // <--- ANIMATION: Drag your Animator component here!

     [Header("Flight Settings")]
     public float flySpeed = 15f;
     public float boostMultiplier = 2f;
     public float rotationSpeed = 3f;
     public float hoverStrength = 5f;


     [Header("Win Condition")]
     public float pollenToWin = 100f; // Amount of pollen needed to win
     public float currentPollen = 0f; // Current amount of pollen collected

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
     private bool isDead = false; // Prevent multiple game over calls

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

          // <--- ANIMATION: Start flapping immediately!
          if (beeAnimator != null)
          {
               beeAnimator.SetBool("isFlying", true);
          }
     }

     void Update()
     {
          if (isDead) return; // Stop inputs if game over

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

          // 3. Handle Stamina Drain & Game Over Check
          HandleStamina();
     }

     void FixedUpdate()
     {
          if (isDead) return;

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

          // --- GAME OVER LOGIC ---
          // If out of energy...
          if (currentStamina <= 0)
          {
               // ...and we have hit the ground (stopped moving)
               if (rb.linearVelocity.magnitude < 0.2f && !isDead)
               {
                    Debug.Log("Bee Exhausted. Game Over.");
                    isDead = true; // Lock controls

                    // <--- ANIMATION: Stop flapping wings (Switch to Idle/Land)!
                    if (beeAnimator != null)
                    {
                         beeAnimator.SetBool("isFlying", false);
                    }

                    if (gameOverManager != null)
                    {
                         gameOverManager.TriggerGameOver();
                    }
               }
          }
     }

     // Called by Flowers to refill energy (Nectar)
     public void RestoreStamina(float amount)
     {
          if (isDead) return; // Can't drink if dead

          currentStamina += amount;
          if (currentStamina > maxStamina) currentStamina = maxStamina;
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