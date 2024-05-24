using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 50f; // Speed of the fireball
    public float lifetime = 5f; // Time in seconds before the fireball is automatically destroyed
    public int damage = 1;
    public Animator animator;
    public AnimationClip explosionAnimationClip;
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
        animator.SetBool("isFlying", true); // Start the flying animation
        Destroy(gameObject, lifetime); // Destroys this game object after 'lifetime' seconds
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Damage the enemy
            SlimeEnemy enemy = collision.GetComponent<SlimeEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Play explosion animation and destroy the fireball
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        // Set flying to false and trigger explosion animation
        animator.SetBool("isFlying", false);
        animator.SetTrigger("Explode");

        // Wait for the exact length of the explosion animation clip
        yield return new WaitForSeconds(explosionAnimationClip.length);
        Debug.Log(explosionAnimationClip.length + "");
        Destroy(gameObject);
    }
}
