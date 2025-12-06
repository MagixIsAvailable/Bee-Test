/*
This script manages the interaction between the player and the hive structure. When the player enters the hive's trigger collider, it calls the GameManager to deposit any collected pollen, effectively banking the player's score. This interaction is crucial for progressing in the game, as it allows players to secure their points and prepare for further resource gathering.
*/



using UnityEngine;

public class HiveScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Call the Manager to bank the points
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DepositPollen();
            }
        }
    }
}