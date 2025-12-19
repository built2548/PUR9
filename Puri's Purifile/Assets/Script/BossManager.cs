using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Movement")]
[SerializeField] private float moveSpeed = 3f;
private Transform player;
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 3f;
    private float nextAttackTime;

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;
    public bool isDead = false;

    [Header("Phase Control")]
    public int currentPhase = 1;
    [Header("Shooting")]
[SerializeField] private GameObject projectilePrefab;
[SerializeField] private Transform firePoint;


    private HealthBar bossHealthBar;
    private Animator anim;
    private Rigidbody2D rb; // Declared here

    public void Shoot() // This MUST be public
{
    if (isDead) return;
    Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
}

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Assigned here
        player = GameObject.FindGameObjectWithTag("Player").transform;

        bossHealthBar = GetComponentInChildren<HealthBar>();
        if (bossHealthBar != null) bossHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

 void Update()
{
    if (isDead || player == null) return;

    // 1. Check if we are currently attacking
    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    if (stateInfo.IsTag("Attack")) 
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        return; 
    }

    // 2. Move toward player
    float direction = Mathf.Sign(player.position.x - transform.position.x);
    float distance = Vector2.Distance(transform.position, player.position);

    if (distance > 3f) // Only walk if far away
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        
        // Flip the boss to face player
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    else 
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // 3. Update Animator Speed
    anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

    // 4. Attack Timer
    if (Time.time >= nextAttackTime)
    {
        ChooseRandomAttack();
        nextAttackTime = Time.time + attackCooldown;
    }
}

    void ChooseRandomAttack()
    {
        int randomAtk = Random.Range(1, 4); 
        anim.SetInteger("AttackType", randomAtk);
        anim.SetTrigger("Attack");
        
        if (currentPhase == 2 && Random.value > 0.7f)
        {
            anim.SetTrigger("Special");
        }

        if (Random.value > 0.8f) 
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isJumping", false);
        }
    }

    public void BossTakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");
        if (bossHealthBar != null) bossHealthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= maxHealth * 0.5f && currentPhase == 1)
        {
            StartPhaseTwo();
        }

        if (currentHealth <= 0) Die();
    }

    void StartPhaseTwo()
    {
        currentPhase = 2;
        attackCooldown *= 0.7f; // Optional: Make boss faster in Phase 2
        anim.SetTrigger("Enrage");
        Debug.Log("Boss enters Phase 2!");
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Dead");
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(5000);
        Destroy(gameObject, 3f);
    }
}