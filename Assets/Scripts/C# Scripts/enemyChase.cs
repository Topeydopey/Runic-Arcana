using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    private Rigidbody2D rb;

    // States
    private enum State
    {
        Idle,
        Chasing
    }

    private State currentState = State.Idle;

    // Distance to start chasing
    public float chaseThreshold = 10f;

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

        // State management
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer < chaseThreshold)
            {
                currentState = State.Chasing;
            }
            else
            {
                currentState = State.Idle;
            }
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Idle:
                Idle();
                break;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 targetPosition = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(targetPosition);
    }

    private void Idle()
    {
        // Implement idle behavior (e.g., do nothing or patrol around)
        // Currently does nothing
    }
}
