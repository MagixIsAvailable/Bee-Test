using UnityEngine;

public class SimpleFlower : MonoBehaviour
{
    public GameObject pollenFxPrefab;
    public AudioClip collectSound; // Drag your Chime here!

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Play Sound
            // We get the AudioSource from the Player
            AudioSource[] sources = other.GetComponents<AudioSource>();

            // We grab the SECOND AudioSource (Index 1) because Index 0 is the buzzing
            // Or we just use PlayClipAtPoint to be safe
            if (collectSound != null)
            {
                // Plays sound at the flower's location
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // 2. FX
            if (pollenFxPrefab != null)
            {
                Instantiate(pollenFxPrefab, transform.position, Quaternion.identity);
            }

            // 3. Logic
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPollen(1);
            }

            gameObject.SetActive(false);
        }
    }
}