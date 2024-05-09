using System.Collections;
using UnityEngine;

public class EnemyChaseTeleport : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    private Rigidbody2D rb;

    // Teleportation effects
    public GameObject teleportInEffectPrefab;  // Effect when the enemy appears
    public GameObject teleportOutEffectPrefab; // Effect when the enemy disappears

    // Teleportation settings
    public float teleportRange = 5.0f;
    public float teleportCooldown = 10.0f;
    private float lastTeleportTime = -10.0f;

    // States
    private enum State
    {
        Idle,
        Chasing,
        Teleporting,
        Stopped // Added stopped state for when hit by fireball
    }

    private State currentState = State.Idle;
    private bool isStopped = false; // Control whether the enemy can move or not

    // Distance to start chasing and teleport threshold
    public float chaseThreshold = 10f;
    public float teleportThreshold = 15f;
    public float stopDuration = 3.0f; // Time in seconds the enemy stops after being hit

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

        if (player != null && !isStopped)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Debug.Log("Distance to player: " + distanceToPlayer); // Log distance

            if (distanceToPlayer < chaseThreshold)
            {
                currentState = State.Chasing;
                Debug.Log("Chasing Player"); // Log state change
            }
            else if (distanceToPlayer > teleportThreshold && Time.time > lastTeleportTime + teleportCooldown)
            {
                currentState = State.Teleporting;
                Debug.Log("Attempting to Teleport"); // Log state change
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
            case State.Teleporting:
                TeleportCloser();
                break;
            case State.Idle:
                Idle();
                break;
        }
    }

    private void ChasePlayer()
    {
        if (!isStopped)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 targetPosition = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
    }

    private void TeleportCloser()
    {
        if (!isStopped)
        {
            Instantiate(teleportOutEffectPrefab, transform.position, Quaternion.identity); // Effect when disappearing

            Vector2 teleportDirection = (player.position - transform.position).normalized;
            Vector2 newPosition = new Vector2(player.position.x, player.position.y) - teleportDirection * teleportRange;

            rb.position = newPosition;
            lastTeleportTime = Time.time;

            Instantiate(teleportInEffectPrefab, newPosition, Quaternion.identity); // Effect when appearing

            Debug.Log("Teleported to: " + newPosition); // Log new position
            currentState = State.Chasing; // Immediately chase after teleporting
        }
    }

    private void Idle()
    {
        // Implement idle behavior (e.g., patrol around)
    }

    // Method to be called by fireball collision
    public void ApplyStopEffect()
    {
        isStopped = true;
        currentState = State.Stopped;
        Invoke("ResumeMovement", stopDuration);
        Invoke("DestroyEnemy", stopDuration + 1.0f); // Destroy the enemy a bit after it resumes movement
    }

    // Resume movement
    void ResumeMovement()
    {
        isStopped = false;
    }

    // Destroy the enemy game object
    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
