using UnityEngine;

public class FollowMouseWithDelay : MonoBehaviour
{
    public float moveSpeed = 20f; // Speed at which the object follows the mouse
    public float launchSpeed = 30f; // Speed when launching forward
    public LayerMask groundLayer;
    public float collisionRadius = 0.245f;
    public float returnSpeed = 25f; // Increased speed for snapping back to the mouse position

    private bool isLaunched = false;
    private Vector3 launchDirection;
    private Vector3 mousePosition;

    void Update()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned or found!");
            return;
        }

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (isLaunched)
        {
            LaunchForward();
        }
        else
        {
            // Move toward the mouse position
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);

            // Perform collision check and update position
            Collider2D hit = Physics2D.OverlapCircle(targetPosition, collisionRadius, groundLayer);
            if (hit == null)
            {
                transform.position = targetPosition;
            }
        }

        // Check for mouse click to launch the object
        if (Input.GetMouseButtonDown(0) && !isLaunched)
        {
            StartLaunch();
        }
    }

    void StartLaunch()
    {
        // Calculate launch direction towards the mouse
        launchDirection = (mousePosition - transform.position).normalized;
        isLaunched = true;
    }

    void LaunchForward()
    {
        // Move forward in the launch direction
        transform.position += launchDirection * launchSpeed * Time.deltaTime;

        // Check if the object is going out of bounds of the camera
        if (IsOutOfBounds())
        {
            // Immediately snap back to the mouse position if out of bounds
            ReturnToMousePosition();
        }

        // Check for collision
        Collider2D hit = Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer);
        if (hit != null)
        {
            // Stop launching and return to the mouse position
            isLaunched = false;
        }
    }

    void ReturnToMousePosition()
    {
        // Move back towards the mouse position with high speed
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, returnSpeed * Time.deltaTime);

        // If it reaches the mouse position, reset the launch state and allow it to launch again
        if (transform.position == mousePosition)
        {
            isLaunched = false;
        }
    }

    // Check if the object is outside the camera view
    bool IsOutOfBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);
        return screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1;
    }
}
