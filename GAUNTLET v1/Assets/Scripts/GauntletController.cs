using UnityEngine;

public class FollowMouseWithDelay : MonoBehaviour
{
    public float moveSpeed = 20f; // Speed at which the object follows the mouse
    public float launchSpeed = 30f; // Speed when launching forward
    public float slamSpeed = 50f; // Speed for slamming down
    public LayerMask groundLayer;
    public float collisionRadius = 0.245f;
    public float returnSpeed = 50f; // Speed for snapping back to the mouse position
    public float launchThreshold = 0.1f; // Minimum distance to launch

    private bool isLaunched = false;
    private bool isSlamming = false;
    private Vector3 launchDirection;
    private Vector3 mousePosition;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned or found!");
        }
    }

    void Update()
    {
        if (mainCamera == null) return;

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (isSlamming)
        {
            SlamDown();
        }
        else if (isLaunched)
        {
            LaunchForward();
        }
        else
        {
            FollowMouse();
        }

        HandleInput();
    }

    private void FollowMouse()
    {
        // Calculate direction from gauntlet to mouse position
        Vector3 directionToMouse = mousePosition - transform.position;

        // Add an offset to this direction so the gauntlet doesn't center directly on the cursor
        float offsetDistance = 0.5f; // This controls how far the gauntlet stays from the cursor
        Vector3 offsetDirection = directionToMouse.normalized * offsetDistance;

        // Calculate the new target position with the offset
        Vector3 targetPosition = mousePosition - offsetDirection;

        // Perform collision check before moving
        if (!Physics2D.OverlapCircle(targetPosition, collisionRadius, groundLayer))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }


    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isLaunched && Vector3.Distance(transform.position, mousePosition) > launchThreshold)
        {
            StartLaunch();
        }

        if (Input.GetMouseButtonDown(1) && !isSlamming) // Right-click for slam
        {
            StartSlam();
        }
    }

    private void StartLaunch()
    {
        launchDirection = (mousePosition - transform.position).normalized;
        isLaunched = true;
    }

    private void LaunchForward()
    {
        transform.position += launchDirection * launchSpeed * Time.deltaTime;

        // Check for collision with the ground to stop launching
        if (Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer))
        {
            isLaunched = false;
        }
    }

    private void StartSlam()
    {
        isSlamming = true;
    }

    private void SlamDown()
    {
        Vector3 slamTarget = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z); // Slam downwards
        transform.position = Vector3.MoveTowards(transform.position, slamTarget, slamSpeed * Time.deltaTime);

        // Check for collision with the ground to stop slamming
        if (Physics2D.OverlapCircle(transform.position, collisionRadius, groundLayer))
        {
            isSlamming = false;
        }
    }
}
