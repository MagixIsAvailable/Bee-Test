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