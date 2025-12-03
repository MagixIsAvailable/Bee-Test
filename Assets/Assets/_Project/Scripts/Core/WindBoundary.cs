using UnityEngine;

public class WindBoundry : MonoBehaviour
public AudioClip windGustSound;    // Sound to play when player hits the wind boundary     
{
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))     // If the object entering the trigger has the tag "Player"
    {
        //1. Play Wind SOUND (Warning Sound)
        AudioSource.PlayClipAtPoint(windGustSound, transform.position);
        //2. Push the player back
        Rigidbody beeRb = other.GetComponent<Rigidbody>();
        if (beeRb != null)
        {
            Vector3 pushDirection = other.transform.position - transform.position; // Calculate direction away from the wind boundary
            pushDirection.y = 0; // Keep the push horizontal
            pushDirection.Normalize(); // Normalize to get a unit vector

            float pushForce = 500f; // Adjust force as needed
            beeRb.AddForce(pushDirection * pushForce); // Apply force to the player's Rigidbody
        }
    }
}
}
