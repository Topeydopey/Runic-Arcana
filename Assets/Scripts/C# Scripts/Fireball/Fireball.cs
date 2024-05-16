using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 50f; // Speed of the fireball
    public float lifetime = 5f; // Time in seconds before the fireball is automatically destroyed

    public void Initialize(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on fireball!");
        }

        Destroy(gameObject, lifetime); // Destroys this game object after 'lifetime' seconds
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy if it has a TakeDamage method
            SlimeEnemy slimeEnemy = other.GetComponent<SlimeEnemy>();
            if (slimeEnemy != null)
            {
                slimeEnemy.TakeDamage(1); // Inflict damage on the slime enemy
            }
            Destroy(gameObject); // Destroy the fireball after hitting the enemy
        }
    }
}
