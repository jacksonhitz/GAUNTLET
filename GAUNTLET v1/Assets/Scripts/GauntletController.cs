using UnityEngine;
using UnityEngine.Tilemaps;

public class GauntletController : MonoBehaviour
{

    float currentSpeed;

    public float moveSpeed = 20f;
    public float punchSpeed = 30f;
    public float punchDistance = 2f;
    public float punchDuration = 0.5f;

    public float slamSpeed = 30f;
    public float slamDuration = 0.2f;

    public Tilemap tilemap;

    public float collisionRadius = 0.245f;

    bool isPunching = false;
    bool isReturning = false;
    bool isSlamming = false;
    bool isShielded = false;
    public GameObject shield;

    Vector3 punchDirection;
    Vector3 mousePosition;
    Camera mainCamera;

    float punchTimer = 0f;
    float slamTimer = 0f;
    Vector3 slamStartPosition;

    Vector3 punchStartPosition;

    BoxCollider2D boxCollider;

    // Add Rigidbody2D here
    private Rigidbody2D rb;

    public int damage = 10; // Damage dealt to enemies

    // Sound effects
    public AudioClip punchSound;
    public AudioClip slamSound;
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        // Initialize Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Corrected line

        // Hide the cursor and allow it to move freely from the start
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None; // Ensure the cursor isn't locked to the center

        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (mainCamera == null) return;

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        boxCollider.enabled = isPunching || isSlamming;

        if (isSlamming && !isShielded)
        {
            SlamDown();
        }
        else if (isPunching && !isShielded)
        {
            PunchForward();
        }
        else
        {
            if (IsClear(mousePosition))
            {
                FollowMouse();
            }
            else
            {
                Idle();
            }
        }

        HandleInput();

        // Check if Escape is pressed to unlock the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            shield.SetActive(true);
            isShielded = true;
        }
        else
        {
            shield.SetActive(false);
            isShielded = false;
        }
    }

    void FollowMouse()
    {
        Vector3 directionToMouse = mousePosition - transform.position;
        float offsetDistance = 0.5f;
        Vector3 offsetDirection = directionToMouse.normalized * offsetDistance;
        Vector3 targetPosition = mousePosition - offsetDirection;

        if (IsClear(targetPosition))
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, currentSpeed * Time.deltaTime));
        }

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    bool IsClear(Vector3 position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(position);
        TileBase tile = tilemap.GetTile(tilePosition);
        return tile == null;
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isPunching && !isReturning && !isSlamming)
        {
            StartPunch();
        }

        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown("x")) && !isSlamming)
        {
            StartSlam();
        }
    }

    void StartPunch()
    {
        punchDirection = (mousePosition - transform.position).normalized;
        punchStartPosition = transform.position;
        punchTimer = 0f;
        isPunching = true;

        PlaySound(punchSound);
    }

    void PunchForward()
    {
        punchTimer += Time.deltaTime;

        // Move the Rigidbody2D instead of directly modifying transform.position
        rb.MovePosition(Vector3.MoveTowards(rb.position, punchStartPosition + punchDirection * punchDistance, punchSpeed * Time.deltaTime));

        if (punchTimer >= punchDuration)
        {
            isPunching = false;
        }
    }

    void StartSlam()
    {
        slamStartPosition = transform.position;
        slamTimer = 0f;
        isSlamming = true;

        PlaySound(slamSound);
    }

    void SlamDown()
    {
        slamTimer += Time.deltaTime;

        // Apply a faster rotation, for example, 300 degrees per second
        transform.Rotate(Vector3.forward * -250 * Time.deltaTime);

        // Move the Rigidbody2D for the slam effect
        rb.MovePosition(Vector3.MoveTowards(rb.position, slamStartPosition - Vector3.up * punchDistance, slamSpeed * Time.deltaTime));

        if (slamTimer >= slamDuration)
        {
            isSlamming = false;
        }
    }

    void Idle()
    {
        transform.Rotate(Vector3.forward * 360 * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPunching || isSlamming)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Method to unlock the cursor and show it again
    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Unset the lock state to allow free cursor movement
    }
}
