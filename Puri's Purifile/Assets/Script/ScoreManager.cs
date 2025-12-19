using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps score object alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadScore(); // Load score at the start of the game
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        SaveScore(); // Save every time the score changes
        UpdateScoreUI();
    }

    public void SaveScore()
    {
        // "CurrentScore" is the key (the name of the save slot)
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.Save(); 
    }

    public void LoadScore()
    {
        // Loads the saved number, or 0 if it's the first time playing
        score = PlayerPrefs.GetInt("CurrentScore", 0);
    }

    public void ResetScore()
    {
        score = 0;
        PlayerPrefs.SetInt("CurrentScore", 0);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public int GetScore() { return score; }
}