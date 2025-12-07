using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene Management

public class MainMenu : MonoBehaviour
{
    public void PlayGame() // Start the game

    {
        SceneManager.LoadScene("MainScene");    // Load the main game scene
    }
    public void QuitGame()      // Quit the application
    {


        Application.Quit();
        Debug.Log("Quit Game"); // Log message for editor

    }
}



