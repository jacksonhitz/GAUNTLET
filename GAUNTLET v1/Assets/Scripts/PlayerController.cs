using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public int maxJumps = 2; // Maximum jumps (1 = single jump, 2 = double jump)\
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

    Animator animator;

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

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("isJumping", !isGrounded);

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
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Set movement velocity

        // Only play walking sound when grounded
        if (moveInput != 0 && isGrounded)
        {
            if (!isMoving)
            {
                Debug.Log("Walking sound should play.");
                PlayWalkingSound();
                isMoving = true;
            }
        }
        else
        {
            if (isMoving && isGrounded)
            {
                Debug.Log("Walking sound should stop.");
                StopWalkingSound();
                isMoving = false;
            }
        }

        // Stop walking sound if in the air
        if (!isGrounded && isMoving)
        {
            StopWalkingSound();
        }

        // Flip the sprite depending on the direction of movement
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
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
            animator.SetBool("isJumping", !isGrounded);
            if (jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply vertical force
                jumpCount++; // Increase jump count

                PlaySound(jumpSound); // Play the jump sound (first or second jump)
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

        // Update Animator
        animator.SetBool("isDashing", true);

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

        // Update Animator
        animator.SetBool("isDashing", false);
        rb.gravityScale = 1; // Restore gravity
    }

    private void PlayWalkingSound()
    {
        if (walkingSound != null && audioSource != null)
        {
            if (audioSource.clip != walkingSound || !audioSource.isPlaying)
            {
                Debug.Log("Playing walking sound.");
                audioSource.Stop(); // Ensure the audio stops before playing
                audioSource.clip = walkingSound;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                Debug.Log("Walking sound already playing.");
            }
        }
        else
        {
            Debug.Log($"Conditions not met: walkingSound={walkingSound}, audioSource={audioSource}, isPlaying={audioSource?.isPlaying}");
        }
    }


    private void StopWalkingSound()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == walkingSound)
        {
            Debug.Log("Stopping walking sound.");
            audioSource.Stop();
            audioSource.clip = null; // Clear the clip to reset
        }
        else
        {
            Debug.Log("StopWalkingSound conditions not met.");
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
