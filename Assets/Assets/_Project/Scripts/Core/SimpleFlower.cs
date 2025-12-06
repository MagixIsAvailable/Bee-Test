/*
This modular script handles all resource gathering by detecting OnTriggerEnter events when the player flies through a specific collider box. It functions as a universal resource node, allowing the developer to define different values for Pollen (Score), Nectar (Stamina), and Water in the Inspector, meaning a single script can power flowers, puddles, or hidden items. Upon triggering, it instantiates visual feedback (Particle Systems), plays a 3D spatial sound effect, updates the GameManager, and then disables itself to prevent infinite collection. This design keeps resource collection logic centralized and easily adjustable for various in-game objects.
*/

using UnityEngine;

public class SimpleFlower : MonoBehaviour
{
    [Header("Behavior")]
    public bool isOneTimeUse = true; // CHECK for Flowers, UNCHECK for Water

    [Header("Visuals & Audio")]
    public GameObject collectFxPrefab; // Pollen=Yellow, Water=Blue
    public AudioClip collectSound;     // Chime=Flower, Splash=Water

    [Header("Resource Values")]
    public int pollenValue = 1;        // Score
    public float nectarValue = 20f;    // Stamina
    public int waterValue = 0;         // Water Count

    // Cooldown to prevent deafening audio if you sit in the puddle
    private float lastCollectTime;
    private float cooldown = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Check Cooldown (Only matters for infinite sources)
        if (Time.time < lastCollectTime + cooldown) return;

        if (other.CompareTag("Player"))
        {
            lastCollectTime = Time.time;

            // 1. Play Sound
            if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // 2. Spawn FX
            if (collectFxPrefab != null) Instantiate(collectFxPrefab, transform.position, Quaternion.identity);

            // 3. Add Resources
            if (GameManager.Instance != null)
            {
                if (pollenValue > 0) GameManager.Instance.AddPollen(pollenValue);
                if (waterValue > 0) GameManager.Instance.AddWater(waterValue);
            }

            // 4. Refill Stamina
            if (nectarValue > 0)
            {
                BeeController bee = other.GetComponent<BeeController>();
                if (bee != null) bee.RestoreStamina(nectarValue);
            }

            // 5. The "Puddle" Logic
            if (isOneTimeUse)
            {
                gameObject.SetActive(false); // Destroy flower
            }
            else
            {
                // Do nothing! The puddle stays there.
                // The 'cooldown' prevents spamming it 100 times a second.
            }
        }
    }
}