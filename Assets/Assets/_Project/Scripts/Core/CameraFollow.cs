using UnityEngine;

public class CameraFollow : MonoBehaviour
{
     public Transform target; // Drag PlayerBee here

     public float mouseSensitivity = 3.0f;
     public float distanceFromTarget = 5.0f;
     public Vector2 pitchLimits = new Vector2(-40, 85); // Stops camera flipping upside down

     private float yaw = 0.0f;
     private float pitch = 0.0f;

     void Start()
     {
          // Initialize rotation to current camera angle
          Vector3 angles = transform.eulerAngles;
          yaw = angles.y;
          pitch = angles.x;
     }

     void LateUpdate()
     {
          if (!target) return;

          // 1. Get Mouse Input
          yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
          pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

          // 2. Clamp Pitch (prevent flipping over)
          pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

          // 3. Calculate Rotation & Position
          Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

          // Position is: Target Position - (Rotation * Distance) + Height Offset
          Vector3 position = target.position - (rotation * Vector3.forward * distanceFromTarget);
          position.y += 1.5f; // Look slightly above the bee's center

          // 4. Apply
          transform.rotation = rotation;
          transform.position = position;
     }
}