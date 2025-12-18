using UnityEngine;

public class ExtraLifeItem : MonoBehaviour
{
    [SerializeField] private int healthAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puri_Script player = other.GetComponent<Puri_Script>();
            if (player != null)
            {
                player.AddLife(healthAmount);
                Destroy(gameObject); // Remove item after pickup
            }
        }
    }
    void Update()
{
    // Gentle floating movement
    float newY = Mathf.Sin(Time.time * 3f) * 0.1f;
    transform.position += new Vector3(0, newY * Time.deltaTime, 0);
}
}