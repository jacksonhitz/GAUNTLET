using UnityEngine;

public class SlimeHealth : MonoBehaviour
{
    public int maxHealth = 20;             // Maximum health of the slime
    public int currentHealth;             // Current health of the slime
    public int damageAmount = 10;         // Amount of damage taken on collision
    public GameObject deathEffectPrefab;  // Optional: Effect to spawn on death

    private bool isDead = false;          // To prevent multiple death calls

    void Start()
    {
        currentHealth = maxHealth;        // Initialize health to max health
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the slime collides with the gauntlets
        if (collision.gameObject.CompareTag("Gauntlet"))
        {
            TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;               // Prevent further damage if already dead

        currentHealth -= damage;          // Reduce health
        Debug.Log($"Slime took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();                        // Handle death
        }
    }

    private void Die()
    {
        if (isDead) return;               // Ensure death logic is called only once
        isDead = true;

        Debug.Log("Slime Died!");

        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            // Handle particle system settings
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.loop = false;                      // Ensure looping is off
                Destroy(effect, ps.main.duration);    // Destroy effect after it completes
            }
            else
            {
                Destroy(effect, 1f);                  // Fallback: Destroy effect after 2 seconds
            }
        }

        Destroy(gameObject); // Destroy the slime object
    }
}
