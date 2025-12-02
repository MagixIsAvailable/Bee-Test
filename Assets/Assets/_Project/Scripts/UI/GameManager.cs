using UnityEngine;
using TMPro; // Needed for UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Makes it easy to talk to from other scripts

    public TextMeshProUGUI scoreText; // Drag your UI Text here
    private int totalPollen = 0;

    void Awake()
    {
        Instance = this; // Sets up the link
    }

    public void AddPollen(int amount)
    {
        totalPollen += amount;

        // Update the screen
        if (scoreText != null)
        {
            scoreText.text = "POLLEN: " + totalPollen;
        }

        Debug.Log("Total Pollen: " + totalPollen);
    }
}