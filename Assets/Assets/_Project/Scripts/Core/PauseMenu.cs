using UnityEngine;                 // Required for Unity engine functionalities
using UnityEngine.SceneManagement; // Required for scene management

public class PauseMenu : MonoBehaviour
{
     [Header("UI References")]
     public GameObject pausePanel; // Reference to the pause menu panel

     private bool isPaused = false; // Tracks the pause state

     void Update()
     {
          // Check for the Escape key press to toggle pause state
          if (Input.GetKeyDown(KeyCode.Escape))
          {
               if (isPaused)
               {
                    Resume();
               }
               else
               {
                    Pause();
               }
          }
     }

     public void Pause()
     {
          pausePanel.SetActive(true); // Show the pause menu
          Time.timeScale = 0f;        // Freeze game time
          isPaused = true;            // Update pause state

          // Optionally, unlock the cursor for menu interaction
          Cursor.lockState = CursorLockMode.None;
          Cursor.visible = true;
     }
     public void Resume()
     {
          pausePanel.SetActive(false); // Hide the pause menu
          Time.timeScale = 1f;         // Resume game time
          isPaused = false;            // Update pause state

          // Optionally, lock the cursor back to the game
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
     }

     public void QuitToMainMenu()
     {
          Time.timeScale = 1f; // Ensure time scale is reset
          SceneManager.LoadScene("MainMenu"); // Load the main menu scene
     }

     public void QuitDesktop()
     {
          Debug.Log("Quitting application..."); // Log quitting action
          Application.Quit(); // Quit the application

     }
}