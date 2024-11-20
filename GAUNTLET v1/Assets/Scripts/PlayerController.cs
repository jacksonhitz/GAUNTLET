using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public int maxJumps = 2; // Maximum jumps (1 = single jump, 2 = double jump)
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpCount;
    private bool isDashing;
    private float dashTime;
    private Vector2 dashDirection;
    private bool canDash; // Tracks if dash is available

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canDash = true; // Dash is initially available
    }

    void Update()
    {
        // Perform a ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleDash();

        // Debugging output to verify ground check and jump count
        Debug.Log($"Grounded: {isGrounded}, JumpCount: {jumpCount}, CanDash: {canDash}");
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        // Reset jump count and dash availability when grounded
        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count on the ground
            canDash = true; // Reset dash on the ground
        }

        // Jump when pressing the jump button and jump count is within limits
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
            }
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && canDash)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            rb.velocity = dashDirection * dashSpeed;

            if (dashTime <= 0)
            {
                EndDash();
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;

        // Get horizontal input for dash direction
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        dashDirection = new Vector2(horizontalInput, 0).normalized;

        // If no direction is input, dash in the current facing direction
        if (dashDirection == Vector2.zero)
        {
            dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        // Disable gravity during dash
        rb.gravityScale = 0;

        // Consume dash
        canDash = false;
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1; // Restore gravity
    }
}
