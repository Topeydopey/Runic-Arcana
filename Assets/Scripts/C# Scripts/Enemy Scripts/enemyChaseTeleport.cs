using System.Collections;
using UnityEngine;

public class EnemyChaseTeleport : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    public float boostSpeed = 10.0f;  // Increased speed for the boost
    public float boostDuration = 2.0f; // Duration of the boost in seconds
    private Rigidbody2D rb;
    private float currentSpeed;
    private float boostEndTime;
    private Vector2 boostTargetPosition; // Target position to boost towards

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
        Boosting, // State during boosting
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
        currentSpeed = speed;
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
            Debug.Log("Distance to player: " + distanceToPlayer); // Log distance continuously

            if (distanceToPlayer < chaseThreshold && currentState != State.Boosting)
            {
                if (currentState != State.Chasing)
                {
                    Debug.Log("Changing State to Chasing");
                    currentState = State.Chasing;
                }
            }
            else if (distanceToPlayer > teleportThreshold && Time.time > lastTeleportTime + teleportCooldown)
            {
                if (currentState != State.Teleporting)
                {
                    Debug.Log("Changing State to Teleporting");
                    currentState = State.Teleporting;
                }
            }
            else if (currentState != State.Boosting && currentState != State.Idle)
            {
                Debug.Log("Changing State to Idle");
                currentState = State.Idle;
            }
        }

        if (Time.time > boostEndTime && currentState == State.Boosting)
        {
            Debug.Log("Boost has ended, changing State to Chasing");
            currentSpeed = speed;  // Reset speed after boost duration
            currentState = State.Chasing; // Continue chasing normally
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Chasing:
            case State.Boosting:
                MoveTowardsTarget();
                break;
            case State.Teleporting:
                TeleportCloser();
                break;
            case State.Idle:
                Idle();
                break;
        }
    }

    private void MoveTowardsTarget()
    {
        if (!isStopped)
        {
            Vector2 targetPosition = currentState == State.Boosting ? boostTargetPosition : (Vector2)player.position;
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, currentSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
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
            boostEndTime = Time.time + boostDuration;  // Set the end time for the boost
            currentSpeed = boostSpeed;  // Set the speed to boost speed
            boostTargetPosition = player.position;  // Set the target position for the boost

            Instantiate(teleportInEffectPrefab, newPosition, Quaternion.identity); // Effect when appearing
            Debug.Log("Teleported to: " + newPosition + " | Boosting to target position."); // Log new position
            currentState = State.Boosting; // Change state to Boosting
        }
    }

    private void Idle()
    {
        // Implement idle behavior (e.g., patrol around)
    }

    public void ApplyStopEffect()
    {
        isStopped = true;
        currentState = State.Stopped;
        Debug.Log("Enemy hit by fireball, stopped.");
        Invoke("ResumeMovement", stopDuration);
        Invoke("DestroyEnemy", stopDuration + 3.0f); // Destroy the enemy a bit after it resumes movement
    }

    void ResumeMovement()
    {
        isStopped = false;
        currentState = State.Idle;  // Ensure state is reset when movement resumes
        Debug.Log("Resuming normal movement.");
    }

    void DestroyEnemy()
    {
        Debug.Log("Destroying enemy.");
        Destroy(gameObject);
    }
}
