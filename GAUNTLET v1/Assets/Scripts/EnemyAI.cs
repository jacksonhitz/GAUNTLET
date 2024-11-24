using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float jumpForce = 4f;
    public float jumpDelay = 0.5f;  // Cooldown between jumps
    public float nodeProx = 0.2f;

    Pathfinding pathfinding;
    List<Vector3> path;
    int targetIndex;
    Rigidbody2D rb;
    public bool canJump = true;  // Variable to track if the enemy can jump

    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        if (path != null && targetIndex < path.Count)
        {
            FollowPath();
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    IEnumerator UpdatePath()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (target != null)
            {
                path = pathfinding.FindPath(transform.position, target.position);
                targetIndex = 0;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void FollowPath()
    {
        Vector3 currentNode = path[targetIndex];
        Vector3 nextNode = (targetIndex + 1 < path.Count) ? path[targetIndex + 1] : currentNode;

        Vector3 direction = nextNode - transform.position;
        direction.z = 0;

        rb.velocity = new Vector2(direction.normalized.x * speed, rb.velocity.y);

        if (direction.magnitude <= nodeProx)
        {
            targetIndex++;
            if (targetIndex >= path.Count)
            {
                targetIndex = path.Count - 1;
            }
        }

        // Jumping logic with cooldown
        if (isGrounded && canJump)
        {
            if (nextNode.y - .5f > transform.position.y)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Debug.Log("jumped");
                StartCoroutine(JumpCooldown());  // Start the cooldown after jumping
            }
        }
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;  // Prevent jumping
        yield return new WaitForSeconds(jumpDelay);  // Wait for the cooldown duration
        canJump = true;   // Allow jumping again
    }
}
