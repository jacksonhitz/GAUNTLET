using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float jumpForce = 4f; 
    public float jumpDelay = 0.5f; 
    public float nodeProx = 0.2f; 

    Pathfinding pathfinding;
    GridManager gridManager;
    List<Vector3Int> path;
    int targetIndex;
    Rigidbody2D rb;
    bool isJumping = false; 

    private void Start()
    {
        pathfinding = GetComponent<Pathfinding>();
        gridManager = FindObjectOfType<GridManager>();
        rb = GetComponent<Rigidbody2D>(); 
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        FollowPath();
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            path = pathfinding.FindPath(transform.position, target.position);
            targetIndex = 0;
            yield return new WaitForSeconds(0.5f); 
        }
    }

    void FollowPath()
    {
        if (path == null || targetIndex >= path.Count) return;

        Vector3 targetPosition = gridManager.TileToWorldPosition(path[targetIndex]);

        if (targetPosition.y > transform.position.y + 0.7f && !isJumping)
        {
            Jump(targetPosition); 
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, targetPosition) < nodeProx)
        {
            targetIndex++; 
        }
    }

    // Jump function to handle upward movement
    void Jump(Vector3 targetPosition)
    {
        isJumping = true; 

        Vector2 jumpDirection = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y).normalized;

        rb.velocity = new Vector2(rb.velocity.x, 0); 
        rb.AddForce(new Vector2(jumpDirection.x * speed, jumpForce), ForceMode2D.Impulse);

        StartCoroutine(ResetJump());
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpDelay); 
        isJumping = false; 
    }
}
