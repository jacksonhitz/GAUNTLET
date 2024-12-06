using UnityEngine;

public class FlyAI : MonoBehaviour
{
    Transform target;
    public float speed = 3f;
    public float followRange = 10f;

    Vector3 initialPosition;

    public int damage = 10; // Damage values

    SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for flipping

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= followRange)
        {
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            FlipEnemy(directionToPlayer.x);
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            Vector3 directionToInitial = (initialPosition - transform.position).normalized;
            FlipEnemy(directionToInitial.x);
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
        }
    }

    void FlipEnemy(float directionX)
    {
        if (directionX > 0)
        {
            // Moving to the right, face right
            spriteRenderer.flipX = false;
        }
        else if (directionX < 0)
        {
            // Moving to the left, face left
            spriteRenderer.flipX = true;
        }
    }

    // Detect collision with the player and apply damage
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Access PlayerHealth script and apply damage
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
