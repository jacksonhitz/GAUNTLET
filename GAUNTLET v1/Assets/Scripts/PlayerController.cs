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

    [Header("Audio")]
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    public bool isGrounded;
    private int jumpCount;
    private bool isDashing;
    private float dashTime;
    private Vector2 dashDirection;
    private bool canDash; // Tracks if dash is available

    private bool isMoving;

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for flipping

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        canDash = true;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if missing
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleDash();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0 && isGrounded)
        {
            if (!isMoving)
            {
                PlayWalkingSound();
                isMoving = true;
            }
        }
        else
        {
            isMoving = false;
            StopWalkingSound();
        }

        // Flip the sprite depending on the direction of movement
        if (moveInput > 0)
        {
            // Moving right
            spriteRenderer.flipX = false; // Make sure the sprite faces right
        }
        else if (moveInput < 0)
        {
            // Moving left
            spriteRenderer.flipX = true; // Flip the sprite to face left
        }
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
            if (jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                PlaySound(jumpSound);
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

        PlaySound(dashSound);
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1; // Restore gravity
    }

    private void PlayWalkingSound()
    {
        if (walkingSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = walkingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == walkingSound)
        {
            audioSource.Stop();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
