using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeProjectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public float lifetime = 5f; // Time in seconds before the projectile is automatically destroyed
    public int damage = 1;
    public Animator animator;
    public AnimationClip explosionAnimationClip;

    private bool canExplode = false; // Flag to indicate if the projectile can explode
    private float activationTime; // Time when the projectile was activated
    private Rigidbody2D rb;
    private Collider2D col;

    public void Initialize(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
            RotateSprite(direction); // Rotate the sprite to face the direction of movement
        }
        else
        {
            Debug.LogError("Rigidbody2D not found on projectile!");
        }
        animator.SetBool("isFlying", true); // Start the flying animation
        Destroy(gameObject, lifetime); // Destroys this game object after 'lifetime' seconds

        activationTime = Time.time; // Record the activation time
        StartCoroutine(EnableExplosionAfterDelay(0.1f)); // Enable explosion after a short delay
    }

    private IEnumerator EnableExplosionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canExplode = true;
    }

    private void RotateSprite(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canExplode) return; // If the explosion is not enabled, do nothing

        if (collision.CompareTag("Player"))
        {
            // Damage the player
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            // Play explosion animation and destroy the projectile
            StartCoroutine(Explode());
        }
        else if (collision.CompareTag("Barrier"))
        {
            // The barrier blocks the projectile
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        // Stop the projectile's movement and disable its collider
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Make the Rigidbody2D kinematic to stop all physics interactions
        }
        if (col != null)
        {
            col.enabled = false;
        }

        // Set flying to false and trigger explosion animation
        animator.SetBool("isFlying", false);
        animator.SetTrigger("Explode");

        // Wait for the exact length of the explosion animation clip
        yield return new WaitForSeconds(explosionAnimationClip.length);
        Debug.Log(explosionAnimationClip.length + "");
        Destroy(gameObject);
    }
}
