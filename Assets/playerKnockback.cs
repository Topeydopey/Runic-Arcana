using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKnockback : MonoBehaviour
{
    public float knockbackForce = 5f;
    public float knockbackRadius = 3f;

    // Update method to trigger knockback
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Trigger knockback with space bar
        {
            ApplyKnockback();
        }
    }

    void ApplyKnockback()
    {
        // Find all colliders within the knockback radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, knockbackRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy")) // Ensure the collider is tagged as "Enemy"
            {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Calculate direction from player to the enemy
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    // Apply force to the enemy
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                    // Additionally, disable the enemy's chase script temporarily
                    enemyChase enemyScript = hit.GetComponent<enemyChase>();
                    if (enemyScript != null)
                    {
                        enemyScript.ApplyKnockback(knockbackDir * knockbackForce);
                    }
                }
            }
        }
    }
}
