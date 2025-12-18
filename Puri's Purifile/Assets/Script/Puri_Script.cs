using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FirstGearGames.SmoothCameraShaker;
using UnityEngine.SceneManagement; // Needed to reload scenes


public class Puri_Script : MonoBehaviour
{
    [Header("Bullet Levels")]
    [SerializeField] private GameObject[] bulletPrefabs;
    private int bulletLevel = 0;
    [Header("UI References")]
    [SerializeField] private BulletLevelUI bulletLevelUI;

    [Header("Audio Child References")]
    [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource shootSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioSource pickupSource;
    [SerializeField] private AudioSource deathSource;

    [Header("Ground Check")]
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] LayerMask whatIsGround;

    [Header("Player Stats")]
    [SerializeField] int maxLives = 3; // Total hearts
    private int currentLives;

    [SerializeField] int attempts = 3; // 💡 NEW: Total Continues
    public int Attempts { get { return attempts; } }

    [SerializeField] float invulnerableDuration = 1.5f;
    private bool isInvulnerable = false;

    [Header("Respawn Logic")]
    private Vector3 currentCheckpoint;

    [Header("Shooting")]
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float fireRate = 0.5f;
    private float nextFireTime;

    [Header("Movement")]
    [SerializeField] float runspeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathkick = new Vector2(1f, 1f);

    // References and Variables
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Vector2 originalScale;
    Animator myAnimator;
    CapsuleCollider2D myCapsule;
    float gravityScaleAtStart;
    bool isAlive = true;
    public ShakeData smallShake;
    private HeartDisplayManager heartDisplayManager;

    void Awake()
    {
        heartDisplayManager = FindObjectOfType<HeartDisplayManager>();
        currentLives = maxLives; // Initialize hearts
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        originalScale = new Vector2(transform.localScale.x, transform.localScale.y);
        myAnimator = GetComponent<Animator>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        if (bulletLevelUI != null)
        {
            bulletLevelUI.UpdateBulletUI(bulletLevel); // Force UI to show Level 1 at start
        }
        if (heartDisplayManager != null)
        {
            heartDisplayManager.UpdateHealthDisplay(currentLives);
        }

        currentCheckpoint = transform.position;

        if (heartDisplayManager != null)
        {
            heartDisplayManager.UpdateHealthDisplay(currentLives);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (!isAlive) { return; }

        if (myCapsule.IsTouchingLayers(LayerMask.GetMask("Ground", "Climbing", "MovingPlatform")) && Mathf.Abs(myRigidbody.linearVelocity.y) <= Mathf.Epsilon)
        {
            myAnimator.SetBool("isJumping", false);
        }

        Run();
        FlipSprite();
        CheckForDeath();
        ClimbLadder();
    }

    // Add this inside Puri_Script.cs
    public void AddLife(int amount)
    {
        // Increase life but don't go over the max
        currentLives = Mathf.Min(currentLives + amount, maxLives);

        // Update the UI hearts
        if (heartDisplayManager != null)
        {
            heartDisplayManager.UpdateHealthDisplay(currentLives);
        }

        // Play pickup sound
        PlayPickupSound();
    }

    // --- CHECKPOINT LOGIC ---
    public void UpdateCheckpoint(Vector3 newPos)
    {
        currentCheckpoint = newPos;
        Debug.Log("Checkpoint Saved!");
    }

    // --- INPUT ACTIONS ---
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!IsGrounded()) { return; }

        if (value.isPressed)
        {
            myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpSpeed);
            myAnimator.SetBool("isJumping", true);
            if (jumpSource != null) jumpSource.Play();
        }
    }

    void OnAttack(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed && Time.time >= nextFireTime)
        {
            myAnimator.SetTrigger("Attack");
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // --- MOVEMENT METHODS ---
    bool IsGrounded()
    {
        Vector2 raycastOrigin = (Vector2)myCapsule.bounds.center + Vector2.down * (myCapsule.size.y / 2f);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, groundCheckDistance, whatIsGround);
        return hit.collider != null;
    }

    void Run()
    {
        if (myAnimator.GetBool("isClimbing"))
        {
            myAnimator.SetBool("isRunning", false);
            return;
        }
        myRigidbody.linearVelocity = new Vector2(moveInput.x * runspeed, myRigidbody.linearVelocity.y);
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x), 1f) * originalScale;
        }
    }

    void ClimbLadder()
    {
        if (!myCapsule.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, moveInput.y * climbSpeed);
        myRigidbody.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    // --- COMBAT METHODS ---
    void Shoot()
    {
        if (bulletPrefabs == null || bulletLevel >= bulletPrefabs.Length || bulletPrefabs[bulletLevel] == null) return;

        if (shootSource != null)
        {
            shootSource.pitch = Random.Range(0.9f, 1.1f);
            shootSource.Play();
        }

        float direction = Mathf.Sign(transform.localScale.x);
        Vector3 spawnPos = transform.position + new Vector3(direction * 0.8f, 0, 0);

        GameObject projectile = Instantiate(bulletPrefabs[bulletLevel], spawnPos, Quaternion.identity);
        PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(direction);
        }
    }

    public void UpgradeBullet()
    {
        if (bulletLevel + 1 < bulletPrefabs.Length)
        {
            bulletLevel++;
            PlayPickupSound();

            // ⭐ NEW: Update the UI display
            if (bulletLevelUI != null)
            {
                bulletLevelUI.UpdateBulletUI(bulletLevel);
            }
        }
    }


    public void ResetBulletLevel()
    {
        bulletLevel = 0;
        if (bulletLevelUI != null)
        {
            bulletLevelUI.UpdateBulletUI(bulletLevel);
        }
    }

    public void PlayPickupSound()
    {
        if (pickupSource != null) pickupSource.Play();
    }

    // --- HEALTH & DEATH LOGIC ---
    void CheckForDeath()
    {
        // Now checks for BOTH Flame and Enemy layers
        if (!isInvulnerable && (myCapsule.IsTouchingLayers(LayerMask.GetMask("Flame", "Enemy"))))
        {
            TakeDamage();
        }
    }

    // --- DAMAGE LOGIC ---
    public void TakeDamage()
    {
        if (!isAlive || isInvulnerable) return;

        currentLives--;
        ResetBulletLevel();
        if (hitSource != null) hitSource.Play();
        CameraShakerHandler.Shake(smallShake);

        if (heartDisplayManager != null)
            heartDisplayManager.UpdateHealthDisplay(currentLives);

        if (currentLives <= 0)
        {
            // Out of hearts? TELEPORT to checkpoint and check Attempts
            StartCoroutine(HandleDeath(true));
        }
        else
        {
            // Still have hearts? DO NOT TELEPORT, just flash
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    // Call this only from your KillZone/Pit script (teleport ALWAYS)
    public void KillInstantly()
    {
        if (!isAlive) return;

        currentLives--; // Lose 1 heart for falling
        ResetBulletLevel();
        if (hitSource != null) hitSource.Play();

        if (heartDisplayManager != null)
            heartDisplayManager.UpdateHealthDisplay(currentLives);

        if (currentLives <= 0)
        {
            StartCoroutine(HandleDeath(true)); // Out of hearts and fell
        }
        else
        {
            StartCoroutine(HandleDeath(false)); // Still have hearts, but TELEPORT because of pit
        }
    }

    IEnumerator HandleDeath(bool useAttempt)
    {
        isAlive = false;
        if (deathSource != null) deathSource.Play();

        myAnimator.SetTrigger("Dying");
        myCapsule.offset = new Vector2(0f, 0.7f);
        myRigidbody.linearVelocity = new Vector2(deathkick.x * -transform.localScale.x, deathkick.y);

        yield return new WaitForSeconds(1.5f);

        if (useAttempt)
        {
            attempts--;
            if (attempts <= 0)
            {
                Debug.Log("GAME OVER - NO ATTEMPTS LEFT!");
                // SceneManager.LoadScene("GameOverScene"); 
                yield break;
            }

            // Reset hearts for the new attempt
            currentLives = maxLives;
            if (heartDisplayManager != null)
                heartDisplayManager.UpdateHealthDisplay(currentLives);
        }

        // --- TELEPORT TO CHECKPOINT ---
        transform.position = currentCheckpoint;
        isAlive = true;

        myRigidbody.linearVelocity = Vector2.zero;
        myRigidbody.gravityScale = gravityScaleAtStart;
        myCapsule.offset = Vector2.zero;

        myAnimator.Play("Idle");
        myAnimator.SetBool("isRunning", false);
        myAnimator.SetBool("isJumping", false);

        StartCoroutine(BecomeTemporarilyInvulnerable());
    }
    IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float flashInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < invulnerableDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true; // Ensure sprite is visible at the end

        isInvulnerable = false;
    }
}