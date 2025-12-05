

// this script spawns objects (like flower petals or quads) in a circular pattern around the GameObject it is attached to.
using UnityEngine;

public class CircularArray : MonoBehaviour
{
    [Header("Settings")]
    public GameObject objectToSpawn; // Drag your Quad or Flower Prefab here
    public int numberOfObjects = 3;  // How many? (2 for X, 3 for Star)
    public float radius = 0f;        // 0 = Flower Mesh. 5 = Fairy Ring.

    [Header("Rotation")]
    public bool alignToCenter = true; // True = Rotate to face outward
    public float rotationOffset = 0f; // Tweak initial angle

    [ContextMenu("Generate Array")] // Adds a button to the component menu
    public void Generate()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Please assign an object to spawn!");
            return;
        }

        // Calculate angle step (e.g., 360 / 3 = 120 degrees)
        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            // 1. Math for positioning in a circle
            float angle = i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;

            // Simple Circle Math: x = cos(a), z = sin(a)
            Vector3 spawnPos = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)) * radius;

            // Add to current position so it spawns around THIS object
            Vector3 finalPos = transform.position + spawnPos;

            // 2. Instantiate
            GameObject newObj = Instantiate(objectToSpawn, finalPos, Quaternion.identity);

            // 3. Parenting (Keep hierarchy clean)
            newObj.transform.SetParent(transform);

            // 4. Rotation Logic
            if (radius == 0)
            {
                // FLOWER MESH MODE: Rotate in place (Star shape)
                // Set rotation based on angle + offset
                newObj.transform.rotation = Quaternion.Euler(0, angle + rotationOffset, 0);
            }
            else if (alignToCenter)
            {
                // FAIRY RING MODE: Look away from center
                newObj.transform.LookAt(transform.position);
                newObj.transform.Rotate(0, 180, 0); // Flip if backwards
            }
        }
    }

    [ContextMenu("Clear Children")] // Button to delete mistakes
    public void ClearChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}