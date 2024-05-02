using System.Collections;
using UnityEngine;

public class enemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private float knockbackEndTime = 0f;
    public float knockbackRecoveryTime = 0.5f;  // Time in seconds the enemy is affected by knockback

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                player = playerGameObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (player != null && Time.time >= knockbackEndTime)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        knockbackEndTime = Time.time + knockbackRecoveryTime;  // Set the time when the enemy can move again
    }
}
