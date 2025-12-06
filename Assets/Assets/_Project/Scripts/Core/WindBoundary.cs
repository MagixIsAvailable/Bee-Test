/* 
Rather than using rigid invisible walls, this script implements an immersive "soft" boundary using physics forces. It utilizes OnTriggerStay to detect when the player attempts to leave the play area, applying a continuous ForceMode.Acceleration to the bee's Rigidbody that pushes them back toward the map center. This simulates a strong headwind that becomes difficult to fight against, accompanied by a looped wind audio cue, providing organic feedback that guides the player back to the playable zone.
*/

using UnityEngine;

public class WindBoundry : MonoBehaviour
{
    public float pushForce = 20f;
    public AudioClip windGustSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Play Wind Sound (Warning)
            AudioSource.PlayClipAtPoint(windGustSound, other.transform.position);

            // 2. Push the Bee Backwards (Opposite to how it entered)
            Rigidbody beeRb = other.GetComponent<Rigidbody>();
            if (beeRb != null)
            {
                // Push back towards the center of the world (0,0,0)
                Vector3 pushDirection = (Vector3.zero - other.transform.position).normalized;
                beeRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            Debug.Log("Turn back! Too windy!");
        }
    }
}
