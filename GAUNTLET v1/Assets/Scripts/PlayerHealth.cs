using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;             // Maximum health points
    public int currentHealth;              // Current health points
    public float damageCooldown = 1f;      // Cooldown time to avoid repeated damage
    public AudioClip damageSound;          // Sound effect for taking damage
    public GameObject damageEffectPrefab;  // Particle effect for taking damage

    private float lastDamageTime = -Mathf.Infinity; // Track time of last damage taken
    private AudioSource audioSource;       // Audio source component

    void Start()
    {
        currentHealth = maxHealth;         // Initialize health
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && damageSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add audio source if missing
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastDamageTime < damageCooldown) return; // Check cooldown
        lastDamageTime = Time.time;

        currentHealth -= damage;           // Reduce health
        PlayDamageFeedback();              // Trigger sound and particle effects

        if (currentHealth <= 0)
        {
            Die();                         // Handle death
        }
    }

    private void PlayDamageFeedback()
    {
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound); // Play damage sound
        }

        if (damageEffectPrefab != null)
        {
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity); // Spawn particle effect
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Add your death logic here (e.g., reload the scene, show game over screen, etc.)
        Destroy(gameObject); // Destroy the player object
    }
}
