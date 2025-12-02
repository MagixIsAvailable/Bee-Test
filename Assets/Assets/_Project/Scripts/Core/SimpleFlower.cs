using UnityEngine;

public class SimpleFlower : MonoBehaviour
{
    // Drag your PollenFx prefab here in the inspector 
    public GameObject pollenFxPrefab;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the colliding object is the Bee (Player)
        if (other.CompareTag("Player"))
        {
            // 2. Instantiate Pollen Effect at flower position
            if (pollenFxPrefab != null)
            {
                Instantiate(pollenFxPrefab, transform.position, Quaternion.identity);
            }

            // 3. Update the Score (Talk to GameManager)
            // This is the part that makes the UI number go up!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPollen(1);
            }

            // 4. Disable flower object (simulates "collecting")
            gameObject.SetActive(false); // <--- FIXED: Added missing semicolon here

            Debug.Log("Flower Pollinated! (+1 Pollen)");
        }
    }
}