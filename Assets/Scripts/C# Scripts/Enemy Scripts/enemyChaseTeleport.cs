using UnityEngine;
using System.Collections;

public class EnemyChaseTeleport : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    public float boostSpeed = 10.0f;
    public float boostDuration = 2.0f;
    private Rigidbody2D rb;
    private float currentSpeed;
    private float boostEndTime;
    private Vector2 boostTargetPosition;

    // Teleportation effects
    public GameObject teleportInEffectPrefab;
    public GameObject teleportOutEffectPrefab;

    // Teleportation settings
    public float teleportRange = 5.0f;
    public float teleportCooldown = 10.0f;
    private float lastTeleportTime = -10.0f;

    // Detection range
    public float detectionRange = 20.0f;

    // Damage settings
    public int damageToPlayer = 1;
    public int maxHealth = 2; // Maximum health
    private int currentHealth;

    // States
    private enum State
    {
        Idle,
        Chasing,
        Teleporting,
        Boosting,
        Stopped
    }

    private State currentState = State.Idle;
    private bool isStopped = false;

    // Distance to start chasing and teleport threshold
    public float chaseThreshold = 10f;
    public float teleportThreshold = 15f;
    public float stopDuration = 3.0f;

    // Animation
    private Animator animator;
    private bool facingRight = true; // Assuming the initial facing direction is right

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
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
            Debug.Log("Distance to player: " + distanceToPlayer);

            if (distanceToPlayer < chaseThreshold && currentState != State.Boosting)
            {
                if (currentState != State.Chasing)
                {
                    Debug.Log("Changing State to Chasing");
                    currentState = State.Chasing;
                    animator.SetBool("isWalking", true);
                }
            }
            else if (distanceToPlayer > teleportThreshold && distanceToPlayer < detectionRange && Time.time > lastTeleportTime + teleportCooldown)
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
                animator.SetBool("isWalking", false);
            }
        }

        if (Time.time > boostEndTime && currentState == State.Boosting)
        {
            Debug.Log("Boost has ended, changing State to Chasing");
            currentSpeed = speed;
            currentState = State.Chasing;
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

            // Flip the enemy based on the movement direction
            FlipSprite(newPosition.x - rb.position.x);
        }
    }

    private void TeleportCloser()
    {
        if (!isStopped)
        {
            Instantiate(teleportOutEffectPrefab, transform.position, Quaternion.identity);

            Vector2 teleportDirection = (player.position - transform.position).normalized;
            Vector2 newPosition = new Vector2(player.position.x, player.position.y) - teleportDirection * teleportRange;

            rb.position = newPosition;
            lastTeleportTime = Time.time;
            boostEndTime = Time.time + boostDuration;
            currentSpeed = boostSpeed;
            boostTargetPosition = player.position;

            Instantiate(teleportInEffectPrefab, newPosition, Quaternion.identity);
            Debug.Log("Teleported to: " + newPosition + " | Boosting to target position.");
            currentState = State.Boosting;
        }
    }

    private void Idle()
    {
        // Implement idle behavior (e.g., patrol around)
    }

    void FlipSprite(float direction)
    {
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public void ApplyStopEffect()
    {
        isStopped = true;
        currentState = State.Stopped;
        animator.SetTrigger("isDamaged");
        Debug.Log("Enemy hit by fireball, stopped.");
        Invoke("ResumeMovement", stopDuration);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");
        // Optionally, disable the teleporter enemy's ability to move or interact
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false; // Disable this script

        // Wait for the death animation to complete before destroying the game object
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageToPlayer);
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the fireball on impact
        }
    }
}
