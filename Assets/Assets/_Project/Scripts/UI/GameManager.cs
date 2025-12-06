/*
The Game Manager serves as the project's Singleton, acting as a global bridge between the UI, the Player, and game world events. It tracks persistent variables such as carriedPollen, bankedScore, and collectedWater, ensuring that game states are preserved across different interactions. It creates the core "Rogue-lite" loop by enforcing a Max Capacity limit, triggering weight penalties on the player when inventory fills, and handling the "Deposit" logic that resets these variables when the player successfully returns to the hive. Additionally, it updates the UI elements in real-time to reflect the player's current status, providing immediate feedback on their progress and encouraging strategic gameplay.
*/

using UnityEngine;
using TMPro;           // Required for TextMeshProUGUI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Drag "POLLEN: 0" text here
    public TextMeshProUGUI hiveText;  // Drag "BANKED: 0" text here 

    [Header("Game Settings")]
    public int maxPollenCapacity = 10;
    public BeeController playerBee;   // Drag  PlayerBee object here

    // Tracking variables
    private int carriedPollen = 0;
    private int bankedPollen = 0;
    private int collectedWater = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPollen(int amount)
    {
        // 1. Check Capacity (The Rogue-lite Limit)
        if (carriedPollen >= maxPollenCapacity)
        {
            Debug.Log("Inventory Full! Return to Hive!");
            return;
        }

        carriedPollen += amount;
        UpdateUI();

        // 2. Slow down the Bee (Physics Interaction)
        UpdateBeeWeight();
    }

    public void AddWater(int amount)                                // New method to add water
    {
        collectedWater += amount;                                  // Increase water count
        Debug.Log("Collected Water: " + collectedWater);          // For debugging
    }

    public void DepositPollen()
    {
        if (carriedPollen > 0)                                              // Only deposit if carrying some
        {
            bankedPollen += carriedPollen;                            // Add to Hive Total
            Debug.Log("Deposited " + carriedPollen + " pollen!");   // For debugging

            carriedPollen = 0; // Empty pockets

            UpdateUI();
            UpdateBeeWeight(); // Reset speed to fast
        }
    }

    // Calculates weight and sends it to the Bee Controller
    void UpdateBeeWeight()
    {
        if (playerBee != null)
        {
            // Ratio is 0.0 (Empty) to 1.0 (Full)
            float ratio = (float)carriedPollen / (float)maxPollenCapacity;
            playerBee.SetEncumbrance(ratio);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "CARRYING: " + carriedPollen + " / " + maxPollenCapacity;  // Update Carrying Text
        }

        // Update Hive Text
        if (hiveText != null)
        {
            hiveText.text = "HIVE TOTAL: " + bankedPollen;
        }
    }
}