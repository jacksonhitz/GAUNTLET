using UnityEngine;

public class FlyAI : MonoBehaviour
{
    Transform target;
    public float speed = 3f;
    public float followRange = 10f;

    Vector3 initialPosition;

    // Damage values
    public int damage = 10;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        initialPosition = transform.position;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= followRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
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
