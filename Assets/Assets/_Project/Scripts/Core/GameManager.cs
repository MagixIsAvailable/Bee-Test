/*
The Game Manager serves as the project's Singleton, acting as a global bridge between the UI, the Player, and game world events. It tracks persistent variables such as carriedPollen, bankedScore, and collectedWater.

UPDATED FEATURES:
- Win Condition: Checks if "bankedPollen" exceeds "pollenGoal" to trigger Victory.
- Rogue-lite Loop: Enforces Max Capacity and Weight Penalties.
*/

using UnityEngine;
using TMPro;           // Required for TextMeshProUGUI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Drag "POLLEN: 0" text here
    public TextMeshProUGUI hiveText;  // Drag "BANKED: 0" text here 
    public TextMeshProUGUI waterText; // Drag "WATER: 0" text here

    [Header("Win Settings")]
    public int pollenGoal = 100;      // <--- SET THIS TO 50 or 100 IN INSPECTOR
    public GameObject winPanel;       // <--- DRAG YOUR NEW "WinPanel" HERE

    [Header("Game Settings")]
    public int maxPollenCapacity = 10;
    public BeeController playerBee;   // Drag PlayerBee object here

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

    public void AddWater(int amount)
    {
        collectedWater += amount;

        // Update the Blue Text on Screen
        if (waterText != null)
        {
            waterText.text = "WATER: " + collectedWater;
        }
    }

    public void DepositPollen()
    {
        if (carriedPollen > 0)
        {
            bankedPollen += carriedPollen;      // Add to Hive Total
            Debug.Log("Deposited " + carriedPollen + " pollen! Total: " + bankedPollen);

            carriedPollen = 0; // Empty pockets

            UpdateUI();
            UpdateBeeWeight(); // Reset speed to fast

            // <--- WIN CONDITION CHECK --->
            if (bankedPollen >= pollenGoal)
            {
                WinGame();
            }
        }
    }

    public void WinGame()
    {
        Debug.Log("VICTORY! Winter is survivable.");

        // 1. Show Mouse Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Freeze Time
        Time.timeScale = 0f;

        // 3. Show Victory Screen
        if (winPanel != null)
        {
            winPanel.SetActive(true);
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
            scoreText.text = "CARRYING: " + carriedPollen + " / " + maxPollenCapacity;
        }

        if (hiveText != null)
        {
            hiveText.text = "HIVE TOTAL: " + bankedPollen + " / " + pollenGoal;
        }
    }
}