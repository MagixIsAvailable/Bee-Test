using UnityEngine;

public class SimpleFlower : MonoBehaviour
{
    // Drag PollenFx pr3efab her in the isnpector 
    public GameObject pollenFxPrefab;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the bee
        if (other.CompareTag("Player"))

        // 2. Instantiate Pollen Effect at flower position
        {
            if (pollenFxPrefab != null)
            {
                Instantiate(pollenFxPrefab, transform.position, Quaternion.identity);
            }
            // 3. Disable flower object (simulates "collecting" the flower)
            gameObject.SetActive(false);

            Debug.Log("Flower Pollinated!(+1 Pollen)");
        }
    }



}
