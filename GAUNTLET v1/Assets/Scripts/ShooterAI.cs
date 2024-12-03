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
    SpriteRenderer spriteRenderer; // Reference to the sprite renderer for flipping the sprite

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the sprite renderer component
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        // Flip the enemy and firepoint based on the player's position
        FlipEnemy();

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
        animator.Play("fire");
        Invoke(nameof(FireProjectile), 0.5f);
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (target.position - firePoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;

        Invoke(nameof(Idle), 0.5f);
    }

    void Idle()
    {
        animator.Play("Idle");
    }

    void FlipEnemy()
    {
        if (target.position.x < transform.position.x)
        {
            // Player is to the left of the enemy, flip the sprite to face left
            spriteRenderer.flipX = false;
            // Move firepoint to the left of the enemy (in front of the enemy's direction)
            firePoint.localPosition = new Vector3(-0.2f, firePoint.localPosition.y, firePoint.localPosition.z);
        }
        else
        {
            // Player is to the right of the enemy, flip the sprite to face right
            spriteRenderer.flipX = true;
            // Move firepoint to the right of the enemy (in front of the enemy's direction)
            firePoint.localPosition = new Vector3(0.2f, firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
