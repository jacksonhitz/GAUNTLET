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

    public GameManagerScript gameManager;
    private bool isDead;
    
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
        PlayDamageFeedback();             

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameManager.gameOver();
        }
    }

    private void PlayDamageFeedback()
    {
        if (damageSound != null && audioSource != null)
        {
            // Temporarily increase the volume before playing the sound
            audioSource.volume = 1.2f; // Slightly increase the volume (e.g., 1.2 is 20% louder)

            audioSource.PlayOneShot(damageSound); // Play the damage sound

            // Optionally reset volume back to the default (1) if you want to prevent this adjustment from affecting other sounds
            audioSource.volume = 1f; // Reset to the default volume
        }

        if (damageEffectPrefab != null)
        {
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity); // Spawn particle effect
        }
    }

}
