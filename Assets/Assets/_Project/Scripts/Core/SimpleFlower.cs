using UnityEngine;

public class SimpleFlower : MonoBehaviour
{
    [Header("Visuals & Audio")]
    public GameObject collectFxPrefab; // Pollen=Yellow, Water=Blue
    public AudioClip collectSound;     // Chime=Flower, Splash=Water

    [Header("Resource Values")]
    // Set these in the Inspector for each type!
    public int pollenValue = 1;        // Score
    public float nectarValue = 20f;    // Stamina
    public int waterValue = 0;         // Water Count (New!)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Play Sound
            if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // 2. Spawn FX
            if (collectFxPrefab != null) Instantiate(collectFxPrefab, transform.position, Quaternion.identity);

            // 3. Add Resources (Only if value > 0)
            if (GameManager.Instance != null)
            {
                if (pollenValue > 0) GameManager.Instance.AddPollen(pollenValue);
                if (waterValue > 0) GameManager.Instance.AddWater(waterValue); // You need to add AddWater to Manager
            }

            // 4. Refill Stamina
            if (nectarValue > 0)
            {
                BeeController bee = other.GetComponent<BeeController>();
                if (bee != null) bee.RestoreStamina(nectarValue);
            }

            // 5. Destroy/Hide
            // If it's a flower, hide it. If it's a pond, maybe don't hide it?
            // For now, let's disable it so you can't collect it infinite times instantly.
            gameObject.SetActive(false);
        }
    }
}