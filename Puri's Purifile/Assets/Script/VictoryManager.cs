using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (ScoreManager.Instance != null && scoreText != null)
        {
            scoreText.text = "Final Score: " + ScoreManager.Instance.GetScore().ToString();
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("CutsceneStage1");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void StartNewGame()
{
    if (ScoreManager.Instance != null)
    {
        ScoreManager.Instance.ResetScore();
    }
    SceneManager.LoadScene("CutsceneStage1");
}
}