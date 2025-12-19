using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        // Unlock and show cursor so player can click buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Display final score from your ScoreManager
        if (ScoreManager.Instance != null && scoreText != null)
        {
            scoreText.text = "Final Score: " + ScoreManager.Instance.GetScore().ToString();
        }
    }

    public void RestartGame()
    {
        // Replace "Level1" with your actual first level name
        SceneManager.LoadScene("Level1");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}