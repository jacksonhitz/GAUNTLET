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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
