using UnityEngine;
using UnityEngine.SceneManagement; // Essential for switching scenes

public class MainMenu : MonoBehaviour
{
[Header("Menu Panels")]
[SerializeField] private GameObject mainMenuPanel;
[SerializeField] private GameObject settingsPanel;

public void OpenSettings()
{
    mainMenuPanel.SetActive(false); // Hide main menu
    settingsPanel.SetActive(true);  // Show settings
}

public void CloseSettings()
{
    settingsPanel.SetActive(false); // Hide settings
    mainMenuPanel.SetActive(true);  // Show main menu again
}
    public void PlayGame()
    {
        // Loads the next scene in the build order
        // Ensure your Game scene is index 1 in Build Settings!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!"); // Shows in console during testing
        Application.Quit();  // Closes the actual .exe file
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0); // Assuming Menu is index 0
    }

}