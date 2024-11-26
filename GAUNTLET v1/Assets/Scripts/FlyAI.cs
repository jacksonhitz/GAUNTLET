using UnityEngine;

public class FlyAI : MonoBehaviour
{
    Transform target;    
    public float speed = 3f; 
    public float followRange = 10f; 

    Vector3 initialPosition;

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
}
