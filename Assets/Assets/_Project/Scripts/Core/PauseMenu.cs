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
               TogglePause();
          }
     }