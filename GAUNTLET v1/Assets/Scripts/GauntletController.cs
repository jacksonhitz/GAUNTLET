using UnityEngine;

public class FollowMouseWithDelay : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask groundLayer;

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 targetPosition = Vector3.Lerp(transform.position, mousePosition, moveSpeed * Time.deltaTime);

        Collider2D hit = Physics2D.OverlapCircle(targetPosition, 0.245f, groundLayer);
        if (hit == null) 
        {
            transform.position = targetPosition;
        }
    }
}
