using UnityEngine;
public class FinalBoss : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;
    private int phase = 1;

    [Header("Phase Settings")]
    [SerializeField] private float speedPhase1 = 2f;
    [SerializeField] private float speedPhase3 = 6f;

    // Use your existing health bar reference
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // Check for Phase Changes
        if (currentHealth <= maxHealth * 0.3f) phase = 3;
        else if (currentHealth <= maxHealth * 0.6f) phase = 2;

        if (currentHealth <= 0) Die();
    }

    void Update()
    {
        // Switch behavior based on phase
        switch(phase)
        {
            case 1: 
                // Normal Attack logic
                break;
            case 2:
                // Faster/Different Attack logic
                break;
            case 3:
                // Rage Mode!
                break;
        }
    }

    void Die()
    {
        // Trigger win screen or explosion
        Debug.Log("Boss Defeated!");
        Destroy(gameObject);
    }
}