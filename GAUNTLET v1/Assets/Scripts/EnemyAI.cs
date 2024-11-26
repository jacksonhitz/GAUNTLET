using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float nodeProx = 0.2f;

    Pathfinding pathfinding;
    List<Vector3> path;
    int targetIndex;
    Rigidbody2D rb;

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
        if (path == null || targetIndex >= path.Count) return;

        Vector3 currentNode = path[targetIndex];

        Vector3 direction = currentNode - transform.position;
        direction.z = 0; 
        rb.velocity = direction.normalized * speed;

        if (direction.magnitude <= nodeProx)
        {
            targetIndex++; 
            if (targetIndex >= path.Count) 
            {
                rb.velocity = Vector2.zero; 
            }
        }
    }
}
