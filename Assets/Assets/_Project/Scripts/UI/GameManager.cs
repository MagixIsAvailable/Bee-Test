using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI scoreText; // Drag UI here
    public TextMeshProUGUI hiveText;  // NEW: Drag a second UI text here for "Banked" score

    private int carriedPollen = 0;
    private int bankedPollen = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddPollen(int amount)
    {
        carriedPollen += amount;
        UpdateUI();
    }

    public void DepositPollen()
    {
        if (carriedPollen > 0)
        {
            bankedPollen += carriedPollen;
            Debug.Log("Deposited " + carriedPollen + " pollen! Total Banked: " + bankedPollen);
            carriedPollen = 0; // Empty the bee's pockets
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "CARRYING: " + carriedPollen + "\nBANKED: " + bankedPollen;
        }
    }
}