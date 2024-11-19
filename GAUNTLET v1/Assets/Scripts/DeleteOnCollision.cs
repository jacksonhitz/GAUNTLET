using UnityEngine;

public class DeleteOnCollision : MonoBehaviour
{
    // This function will be triggered when the object collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object colliding with this is the gauntlet from the previous script
        if (collision.gameObject.CompareTag("Gauntlets"))
        {
            // Destroy this object (the object this script is attached to)
            Destroy(gameObject);
        }
    }

    // Alternatively, if you're using triggers (OnTriggerEnter2D instead of OnCollisionEnter2D)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gauntlets"))
        {
            // Destroy the object if it hits the gauntlet
            Destroy(gameObject);
        }
    }
}
