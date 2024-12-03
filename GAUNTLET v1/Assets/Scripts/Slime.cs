using UnityEngine;

public class SlimeMonster : MonoBehaviour
{
    public float hopForce = 5f;                // The force applied when hopping
    public float detectionRange = 10f;         // Range at which the slime detects the player
    public float cooldownTime = 1f;            // Cooldown time before the slime can hop again
    public LayerMask groundLayer;              // Ground layer to check for collisions
    public AudioClip hopSound;                 // Sound effect for hopping
    public AudioClip hitSound;                 // Sound effect for hitting the player or gauntlets
    public AudioClip deathSound;               // Sound effect for death (final hit)
    public GameObject hitEffectPrefab;         // Particle effect when hitting the player or gauntlets

    private Transform player;                  // Reference to the player's transform
    private bool isHopping = false;            // Whether the slime is currently hopping
    private bool isGrounded = false;           // Whether the slime is currently touching the ground
    private float lastHopTime = 0f;            // Time of the last hop to manage cooldown
    private int health = 100;                  // Slime's health
    private bool isDead = false;               // Whether the slime is dead

    private Rigidbody2D rb;                    // Reference to the slime's Rigidbody2D
    private AudioSource audioSource;           // Audio source component

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assume player is tagged as "Player"
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add audio source if missing
        }
    }

    void Update()
    {
        if (player == null || isDead) return; // Ensure the player reference is set and slime is not dead

        // Hop towards the player if conditions are met
        if (isGrounded && !isHopping && Time.time - lastHopTime >= cooldownTime && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            HopTowardsPlayer();
        }

        // Stop sliding if not hopping, but only when grounded
        if (!isHopping && isGrounded)
        {
            rb.velocity = Vector2.zero; // Zero velocity only when grounded and not hopping
        }
    }

    void HopTowardsPlayer()
    {
        isHopping = true;
        lastHopTime = Time.time;

        // Calculate the direction towards the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 hopDirection = new Vector2(directionToPlayer.x, directionToPlayer.y + 0.5f).normalized;

        rb.velocity = hopDirection * hopForce; // Apply hop force

        PlayHopSound(); // Play hop sound
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for ground collision
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            isHopping = false;
        }

        // Check for player collision
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Inflict damage
                PlayHitFeedback(); // Trigger sound and effects
            }
        }

        // Check for gauntlet collision
        if (collision.gameObject.CompareTag("Gauntlet"))
        {
            PlayHitFeedback(); // Trigger hit feedback on gauntlet collision
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    private void PlayHopSound()
    {
        if (hopSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hopSound); // Play hop sound
        }
    }

    private void PlayHitFeedback()
    {
        // Only play hit sound if slime is not dead
        if (hitSound != null && !isDead && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound); // Play hit sound
        }

        if (hitEffectPrefab != null && !isDead)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity); // Spawn hit particle effect
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Don't take damage if already dead

        health -= damage;
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound); // Play death sound
        }

        // Additional death logic such as playing death animation or despawning
        Destroy(gameObject, 2f); // Destroy the slime after 2 seconds
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
