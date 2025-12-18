using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Sprite activeSprite; 
    
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private int flashCount = 4;

    [Header("Audio")]
    [SerializeField] private AudioSource checkpointSound; 

    private SpriteRenderer sr;
    private bool isActivated = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            
            if (player != null)
            {
                player.UpdateCheckpoint(transform.position);
                ActivateCheckpoint();
            }
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;
        
        // 1. Change the sprite permanently
        if (sr != null && activeSprite != null)
        {
            sr.sprite = activeSprite;
        }

        // 2. ⭐ Start the Flash Effect
        StartCoroutine(FlashEffect());

        // 3. Play sound
        if (checkpointSound != null)
        {
            checkpointSound.Play();
        }

        Debug.Log("Checkpoint Activated!");
    }

    IEnumerator FlashEffect()
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;

        for (int i = 0; i < flashCount; i++)
        {
            // Set to flash color (White)
            sr.color = flashColor;
            yield return new WaitForSeconds(flashInterval);

            // Back to normal
            sr.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
        
        // Ensure it ends on the correct color
        sr.color = originalColor;
    }
}