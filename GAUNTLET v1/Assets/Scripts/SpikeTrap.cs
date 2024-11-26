using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    // Reference to the GameManagerScript
    private GameManagerScript gameManager;

    private void Start()
    {
        // Find the GameManager in the scene and get its script
        gameManager = FindObjectOfType<GameManagerScript>();
        if (gameManager == null)
        {
            Debug.LogError("GameManagerScript not found in the scene!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with the spike trap
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call the Game Over UI from the GameManager
            if (gameManager != null)
            {
                gameManager.gameOver();
            }
        }
    }
}
