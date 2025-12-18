using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Removed "using ScoreSystem" unless you specifically created that namespace
public class ScoreItem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check for the Player tag
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            
            if (player != null)
            {
                // 2. Play the sound effect
                player.PlayPickupSound();
                
                // 3. Award the points through the Singleton
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(scoreValue);
                }
                else
                {
                    Debug.LogWarning("ScoreManager instance not found in scene!");
                }
                
                // 4. Remove the item from the world
                Destroy(gameObject);
            }
        }
    }
}