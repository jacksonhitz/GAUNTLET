using UnityEngine;

public class SlimeMonster : MonoBehaviour
{
    public float hopForce = 5f;                // The force applied when hopping
    public float detectionRange = 10f;         // Range at which the slime detects the player
    public float cooldownTime = 1f;            // Cooldown time before the slime can hop again
    public LayerMask groundLayer;              // Ground layer to check for collisions

    private Transform player;                  // Reference to the player's transform
    private bool isHopping = false;            // Whether the slime is currently hopping
    private bool isGrounded = false;           // Whether the slime is currently touching the ground
    private float lastHopTime = 0f;            // Time of the last hop to manage cooldown

    private Rigidbody2D rb;                    // Reference to the slime's Rigidbody2D

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assume player is tagged as "Player"
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        if (player == null) return; // Ensure the player reference is set

        // If the player is within the slime's detection range and the slime is grounded,
        // make it hop towards the player
        if (isGrounded && !isHopping && Time.time - lastHopTime >= cooldownTime && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            HopTowardsPlayer();
        }

        // If the slime is not hopping, stop sliding
        if (!isHopping)
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Method to make the slime hop towards the player
    void HopTowardsPlayer()
    {
        isHopping = true;
        lastHopTime = Time.time;

        // Calculate the direction towards the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Add an upwards component to the movement direction (diagonal movement)
        Vector2 hopDirection = new Vector2(directionToPlayer.x, directionToPlayer.y + 0.5f).normalized; // Adjust the Y component as needed for the jump height

        // Apply hop force (velocity) towards the player with an upwards angle
        rb.velocity = hopDirection * hopForce;
    }

    // Detect when the slime touches the ground (grounded detection)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the slime collides with the ground
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true; // The slime is on the ground
            isHopping = false; // Reset hopping state
        }
    }

    // Detect when the slime is no longer touching the ground
    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the slime is no longer colliding with the ground
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false; // The slime is in the air
        }
    }

    // Optional: Visual debugging in the scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
