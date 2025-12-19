using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScirpt : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene2" || SceneManager.GetActiveScene().name == "MenuScene")
        {
            Destroy(gameObject);
        }
    }
}