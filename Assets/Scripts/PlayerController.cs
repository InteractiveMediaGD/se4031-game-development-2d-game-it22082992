using UnityEngine;
using UnityEngine.UI; 
using TMPro;           
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image healthFillImage; 
    public Gradient healthGradient; 

    [Header("Movement Settings")]
    public float currentSpeed = 5f;
    public float acceleration = 0.1f;
    public float verticalSpeed = 5f;

    [Header("Visual Settings")]
    public SpriteRenderer playerSpriteRenderer; // Drag the Player's SpriteRenderer here
    public Sprite upSprite;   // Drag the "Fly Up" character sprite here
    public Sprite downSprite; // Drag the "Fly Down" character sprite here
    public Sprite idleSprite; // Drag your standard character sprite here
    public Sprite badMoodSprite; // Drag the "Bad Mood" character sprite here
    public float badMoodDuration = 1.0f; 
    private float badMoodTimer = 0f;

    [Header("Score Settings")]
    public int score = 0;
    public TextMeshProUGUI scoreDisplay;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;      
    public TextMeshProUGUI finalScoreText; 

    public static int finalOverallScore;

    [Header("Effects Settings")]
    public GameObject starParticlePrefab;

    [Header("Audio Settings")]
    public AudioClip hitSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyKilledSound;
    public AudioClip shootSound;
    public AudioClip scoreSound;
    public AudioClip healthPackSound;
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private AudioSource musicSource;

    private bool isDead = false;

    void Start()
    {
        Time.timeScale = 1f; 
        currentHealth = maxHealth;
        
        if(healthSlider != null) healthSlider.maxValue = maxHealth;
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
        
        audioSource = gameObject.AddComponent<AudioSource>();
        
        if (backgroundMusic != null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = true;
            musicSource.Play();
        }
        
        UpdateUI(); 
        AnchorUI();
    }

    void AnchorUI()
    {
        // Anchor Health Bar to Top-Left
        if (healthSlider != null)
        {
            RectTransform rt = healthSlider.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(20, -20); // 20 units padding from top-left
            }
        }

        // Anchor Score Text to Top-Left (below Health Bar)
        if (scoreDisplay != null)
        {
            RectTransform rt = scoreDisplay.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(20, -60); // 60 units padding from top-left
            }
        }
    }

    void Update()
    {
        if (isDead) return; 

        // Automatic forward movement
        currentSpeed += acceleration * Time.deltaTime;
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        // Vertical movement logic
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * verticalInput * verticalSpeed * Time.deltaTime);

        // Update player sprite based on vertical movement direction
        HandlePlayerVisuals(verticalInput);

        // Keep player within camera bounds
        ClampToScreen();

        // Update bad mood timer
        if (badMoodTimer > 0)
        {
            badMoodTimer -= Time.deltaTime;
        }

        // Shooting logic
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void ClampToScreen()
    {
        if (Camera.main == null) return;

        // Get the current screen boundaries in world space
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));

        // Adding some character-size padding (approximate)
        float padding = 0.5f; 

        // Clamp positions
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + padding, maxBounds.y - padding);
        
        // Update position
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    // Swaps sprites to show the character looking up or down
    void HandlePlayerVisuals(float verticalInput)
    {
        if (playerSpriteRenderer == null) return;

        // Visual feedback for being hit (Bad Mood)
        if (badMoodTimer > 0 && badMoodSprite != null)
        {
            playerSpriteRenderer.sprite = badMoodSprite;
            return;
        }

        if (verticalInput > 0.1f)
        {
            playerSpriteRenderer.sprite = upSprite;
        }
        else if (verticalInput < -0.1f)
        {
            playerSpriteRenderer.sprite = downSprite;
        }
        else
        {
            playerSpriteRenderer.sprite = idleSprite;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Projectile proj = bullet.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.speed += currentSpeed; 
            }

            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            TakeDamage(20);

            // Requirement: Camera Shake and Audio on Obstacle hit
            if (other.CompareTag("Obstacle"))
            {
                CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
                if (cam != null) cam.TriggerShake(0.2f, 0.3f);
                
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }

                // Trigger Bad Mood visual
                badMoodTimer = badMoodDuration;
            }
            else if (other.CompareTag("Enemy"))
            {
                if (enemyHitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(enemyHitSound);
                }
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("HealthPack"))
        {
            Heal(15);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ScoreZone"))
        {
            AddScore(10); 
            
            if (scoreSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(scoreSound);
            }

            // Destroys the star on collision
            if (starParticlePrefab != null)
            {
                Instantiate(starParticlePrefab, other.transform.position, Quaternion.identity);
            }
            Destroy(other.gameObject); 
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateUI();

        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            TriggerGameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();

        if (healthPackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healthPackSound);
        }
    }

    public void PlayEnemyKilledSound()
    {
        if (enemyKilledSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(enemyKilledSound);
        }
    }

    void TriggerGameOver()
    {
        isDead = true;
        finalOverallScore = score; // Save score before scene change
        Time.timeScale = 1f; 
        SceneManager.LoadScene("GameOver"); // Ensure scene index 2 is named correctly
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("SampleScene"); 
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    void UpdateUI()
    {
        if (scoreDisplay != null) scoreDisplay.text = "Score: " + score;
        
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            if (healthFillImage != null)
            {
                float healthPercent = (float)currentHealth / maxHealth;
                healthFillImage.color = healthGradient.Evaluate(healthPercent);
            }
        }
    }
}