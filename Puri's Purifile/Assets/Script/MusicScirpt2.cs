using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScirpt2 : MonoBehaviour
{
   private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            Destroy(gameObject);
        }
    }
}
