using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public bool isPlayerBullet = true;

    void Start()
    {
        // Destroy the bullet after 2 seconds so it doesn't lag the game
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Requirement 4 & 5: Destroy on collision
        if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            // Requirement 3: Score is not getting updated when a enemy got shot
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.AddScore(10);
                player.PlayEnemyKilledSound();
            }
            
            // If player hits enemy, kill enemy and destroy bullet
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && other.CompareTag("Player"))
        {
            // If enemy hits player, damage player and destroy bullet
            other.GetComponent<PlayerController>().TakeDamage(10);
            Destroy(gameObject);
        }
    }
}