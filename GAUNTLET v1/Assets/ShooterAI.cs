using UnityEngine;

public class LizardAI : MonoBehaviour
{
    public GameObject projPrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    Transform target;
    public float detectionRange = 10f;
    public float attackInterval = 2f;

    float attackTimer;

    Animator animator;

    private bool hasFiredOnce = false; // Track if the fire animation has played once

    void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float targetCheck = Vector2.Distance(transform.position, target.position);
        if (targetCheck <= detectionRange)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Fire();
                attackTimer = attackInterval;
            }
        }
    }

    void Fire()
    {
        if (!hasFiredOnce)
        {
            animator.Play("fire");  // Play the fire animation once
            animator.speed = 0f;     // Stop the animation from looping immediately
            hasFiredOnce = true;     // Set flag to prevent repeating the animation
            Invoke(nameof(ResetAnimationSpeed), 0.5f);  // Reset animation speed after a short delay
        }

        Invoke(nameof(FireProjectile), 0.5f); // Delay the projectile firing slightly
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (target.position - firePoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }

    // This method resets the animation speed back to normal
    void ResetAnimationSpeed()
    {
        animator.speed = 1f;   // Reset speed to normal
        hasFiredOnce = false;  // Allow the fire animation to play again on the next attack
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
