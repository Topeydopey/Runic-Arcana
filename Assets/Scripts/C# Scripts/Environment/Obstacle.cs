using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fireball"))
        {
            Destroy(gameObject); // Destroy the obstacle
        }
    }
}
