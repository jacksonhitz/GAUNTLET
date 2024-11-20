using UnityEngine;

public class FollowMouseWithDelay : MonoBehaviour
{
    public float moveSpeed = 20f; // Speed at which the object follows the mouse
    public float launchSpeed = 30f; // Speed when launching forward
    public LayerMask groundLayer;
    public float collisionRadius = 0.245f;
    public float returnSpeed = 25f; // Increased speed for snapping back to the mouse position
    public float launchThreshold = 0.1f; // Minimum distance to launch (prevents freezing)

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

        // Check for mouse click to launch the object, but only if it's not already near the mouse
        if (Input.GetMouseButtonDown(0) && !isLaunched && Vector3.Distance(transform.position, mousePosition) > launchThreshold)
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
            // Bounce off the camera borders
            BounceOffBorders();
        }

        // Check for collision with the ground
        Collider2D hit = Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer);
        if (hit != null)
        {
            // Stop launching and allow object to settle
            isLaunched = false;
        }
    }

    void BounceOffBorders()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        // Reverse direction when hitting camera borders
        if (screenPos.x < 0 || screenPos.x > 1)
        {
            launchDirection.x = -launchDirection.x; // Bounce off the left or right
        }

        if (screenPos.y < 0 || screenPos.y > 1)
        {
            launchDirection.y = -launchDirection.y; // Bounce off the top or bottom
        }
    }

    // Check if the object is outside the camera view
    bool IsOutOfBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        // If the object is outside the camera's view (left, right, top, or bottom edges)
        return screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1;
    }
}
