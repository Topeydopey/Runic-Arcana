using UnityEngine;

public class WaterWave : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10; // Adjust damage value as needed

    private Vector2 direction;

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
        Destroy(gameObject, 3.2f); // Destroy after 6 seconds
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Damage the enemy
            SlimeEnemy slimeEnemy = collision.GetComponent<SlimeEnemy>();
            FireSlimeEnemy fireSlimeEnemy = collision.GetComponent<FireSlimeEnemy>();
            TeleportingSlimeEnemy teleportingSlimeEnemy = collision.GetComponent<TeleportingSlimeEnemy>(); // Check for TeleportingSlimeEnemy

            if (slimeEnemy != null)
            {
                slimeEnemy.TakeDamage(damage);
            }
            else if (fireSlimeEnemy != null)
            {
                fireSlimeEnemy.TakeDamage(damage);
            }
            else if (teleportingSlimeEnemy != null) // Apply damage to TeleportingSlimeEnemy
            {
                teleportingSlimeEnemy.TakeDamage(damage);
            }
        }
    }
}
