using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Make sure you have the Input System package

public class PauseMenu : MonoBehaviour
{
    // Static variable so other scripts (like your Player) can check if the game is paused
    public static bool isPaused = false;

    [SerializeField] private GameObject pausePanel;

    void Update()
    {
        // Detects the Escape key press
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resumes physics and animations
        isPaused = false;
        
        // Optional: Hide and lock the cursor when playing
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Freezes physics and animations
        isPaused = true;

        // Optional: Show and unlock the cursor to click menu buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // ALWAYS reset timeScale before changing scenes
        SceneManager.LoadScene("MenuScene"); 
    }
}