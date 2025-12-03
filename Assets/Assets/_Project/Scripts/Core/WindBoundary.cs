using UnityEngine;

public class WindBoundry : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))     // If the object entering the trigger has the tag "Player"
        {//1.
            Destroy(other.gameObject);
        }
    }
}
