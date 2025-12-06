/*
This modular script handles all resource gathering interactions via OnTriggerEnter events. It functions as a universal resource node, allowing the developer to define specific values for Pollen (Score), Nectar (Stamina), and Water directly in the Inspector. The script manages the object's lifecycle through a flexible state system: it supports single-use items (permanently destroyed), renewable resources (respawning via Coroutines after a set duration), and infinite sources (like water puddles) that remain active with a short cooldown. Upon interaction, it instantiates particle effects, triggers spatial audio, and synchronizes data with the global GameManager
*/

using UnityEngine;
using System.Collections; // Required for timers (Coroutines)

public class SimpleFlower : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnTime = 60f; // Time in seconds (60 = 1 min). Set to 0 for Puddles.
    public bool destroyPermanently = false; // Check this for "One Time Only" items

    [Header("Visuals & Audio")]
    public GameObject collectFxPrefab;
    public AudioClip collectSound;

    [Header("Resource Values")]
    public int pollenValue = 1;
    public float nectarValue = 20f;
    public int waterValue = 0;

    // Internal tracking
    private Collider myCollider;
    private Renderer[] myRenderers; // Finds visuals even in child objects
    private float lastCollectTime;
    private float puddleCooldown = 1.0f; // Prevents puddles spamming sound

    void Start()
    {
        myCollider = GetComponent<Collider>();
        // Grab all renderers (including the flower petals inside)
        myRenderers = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Puddle Cooldown Check (Only matters if respawnTime is 0)
        if (respawnTime == 0 && Time.time < lastCollectTime + puddleCooldown) return;

        if (other.CompareTag("Player"))
        {
            lastCollectTime = Time.time;

            // 2. Play Feedback
            if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);
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

            // 5. Handle Disappearing / Respawning
            if (destroyPermanently)
            {
                gameObject.SetActive(false); // Gone forever
            }
            else if (respawnTime > 0)
            {
                StartCoroutine(RespawnRoutine()); // Start the timer
            }
            // If respawnTime is 0 (Puddle), do nothing, just stay there.
        }
    }

    // The Timer Logic
    IEnumerator RespawnRoutine()
    {
        // A. Turn off Visuals and Collision (Poof!)
        if (myCollider) myCollider.enabled = false;
        foreach (var r in myRenderers) r.enabled = false;

        // B. Wait for X seconds
        yield return new WaitForSeconds(respawnTime);

        // C. Turn them back on (Respawn!)
        if (myCollider) myCollider.enabled = true;
        foreach (var r in myRenderers) r.enabled = true;

        // Optional: Play a tiny "Pop" sound or effect here if you want!
    }
}