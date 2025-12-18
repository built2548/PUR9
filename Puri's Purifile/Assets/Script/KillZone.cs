using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool killInstantly = false; // True = Game Over, False = Lose 1 Life & Respawn

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Puri_Script player = other.GetComponent<Puri_Script>();
        if (player != null)
        {
            player.KillInstantly(); // This forces the teleport!
        }
    }
}
}