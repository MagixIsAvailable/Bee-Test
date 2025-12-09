/*
This script manages the game's fail state, conceptually framed as "Exhaustion" rather than death to fit the serious game theme. Upon triggering (when Stamina reaches zero and the player is grounded), it sets Time.timeScale to zero to freeze the physics simulation and unlocks the cursor for UI interaction. Crucially, it integrates the project's educational goals by randomly selecting a string from a beeFacts array to display on the end screen, turning failure into a learning opportunity before handling the scene reload via SceneManager.
*/

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Needed to reload level

public class GameOverManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject gameOverPanel;    // Reference to the game over panel

    public GameObject winPanel;     // Reference to the win panel
    public TextMeshProUGUI factText;   // Reference to the fact text UI element


    [Header("Data")]
    private string[] beeFacts = new string[]
    {
        "Honey bees fly about 15 mph—faster than most people can run!",
        "To make 1lb of honey, bees visit 2 million flowers.",
        "Bees use the sun as a compass, even when it's cloudy.",
        "A bee's wings beat 200 times per second.",
        "Bees can see Ultraviolet (UV) light to find nectar.",
        "A hive's queen can live for several years.",
        "Worker bees effectively work themselves to death in 6 weeks during summer.",
        "Bees perform a 'waggle dance' to share map coordinates.",
        "Honey never spoils; 3,000-year-old honey is still edible!",
        "Male bees (drones) have no stingers and do no work."
        "Bees have 5 eyes: two large compound eyes and three simple eyes (ocelli).",
        "A single bee produces only 1/12th of a teaspoon of honey in her lifetime.",
        "Bees can recognize individual human faces.",
        "Bees communicate using pheromones (scents) as well as dancing.",
        "Only female bees have stingers; males cannot sting.",
        "Honey bees are the only insect that produces food eaten by humans.",
        "Bees pollinate 80% of the world's flowering crops.",
        "A bee's brain is the size of a sesame seed, yet they can learn and remember.",
        "Bees can sense the electric field of a flower to tell if it has nectar.",
        "During winter, bees huddle in a ball to keep the queen warm at 93°F (34°C).",
        "Bees have been producing honey for at least 150 million years.",
        "A colony can contain up to 60,000 bees at its peak.",
        "Bees sleep! They take naps in the hive to consolidate memories.",
        "If the queen dies, workers can create a new queen by feeding a larva 'Royal Jelly'.",
        "Bees are not aggressive; they usually only sting to defend their hive."
    };

    public void TriggerGameOver()
    {
        // 1. Show Cursor so player can click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Pause the game physics
        Time.timeScale = 0f;

        // 3. Pick a random fact
        string randomFact = beeFacts[Random.Range(0, beeFacts.Length)];
        if (factText != null) factText.text = "\"" + randomFact + "\"";

        // 4. Show Screen
        gameOverPanel.SetActive(true);
    }
    public void TriggerWin()
    {
        // 1. Show Cursor so player can click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Pause the game physics
        Time.timeScale = 0f;

        // 3. Pick a random fact
        string randomFact = beeFacts[Random.Range(0, beeFacts.Length)];
        if (factText != null) factText.text = "\"" + randomFact + "\"";

        // 4. Show Win Screen
        winPanel.SetActive(true);


    }
    public void RestartGame()
    {
        // Unpause time
        Time.timeScale = 1f;

        // Reload the current level 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Unpause time
        Time.timeScale = 1f;

        // Quit application
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}