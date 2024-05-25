using UnityEngine;
using System.Collections;

public class SlimeEnemy : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 2.0f;
    public float leapSpeed = 5.0f;
    public float moveSpeed = 1.0f;
    public int maxHealth = 2;  // Maximum health
    private int currentHealth;
    private Transform player;
    private Vector2 randomDirection;
    private float moveInterval = 2.0f; // Interval in seconds between direction changes
    private bool isLeaping = false;
    private Animator animator;
    private bool facingRight = true; // Assuming the initial facing direction is right
    private bool isDying = false; // Flag to check if the slime is dying
    private Rigidbody2D rb;

    private float damageCooldown = 1.0f; // Cooldown period between damage applications
    private float lastDamageTime; // Timestamp of the last damage application

    public AudioClip[] damageSounds; // Array of audio clips for damage sounds
    private AudioSource audioSource; // Audio source component

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Set collision detection mode to continuous
        currentHealth = maxHealth;
        InvokeRepeating("ChangeDirection", 0, moveInterval);
        lastDamageTime = -damageCooldown; // Initialize to ensure immediate damage on first contact

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    void Update()
    {
        if (isDying) return; // If dying, don't perform any other actions

        if (!isLeaping)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRange)
            {
                StartCoroutine(LeapAtPlayer());
            }
            else
            {
                MoveRandomly();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the fireball upon collision
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Continuously damage the player if staying in collision with a cooldown
            ApplyDamage(collision.gameObject);
        }
    }

    void ApplyDamage(GameObject player)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1);
            lastDamageTime = Time.time; // Update the last damage time
            Debug.Log("Player damaged by slime");
        }
    }

    void ChangeDirection()
    {
        randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        FlipSprite(randomDirection.x);
    }

    void MoveRandomly()
    {
        rb.MovePosition(rb.position + randomDirection * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator LeapAtPlayer()
    {
        isLeaping = true;
        animator.SetBool("IsLeaping", true);

        // Stop for a moment before leaping
        yield return new WaitForSeconds(0.5f);
        Vector3 leapDirection = (player.position - transform.position).normalized;
        FlipSprite(leapDirection.x);

        float leapDuration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < leapDuration)
        {
            rb.MovePosition(rb.position + (Vector2)leapDirection * leapSpeed * Time.fixedDeltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop for 1 second after leaping
        yield return new WaitForSeconds(1.0f);

        animator.SetBool("IsLeaping", false);
        isLeaping = false;
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

    public void TakeDamage(int damage)
    {
        if (isDying) return; // If dying, don't take any more damage

        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");

        PlayDamageSound(); // Play a damage sound

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    void PlayDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            AudioClip clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator Die()
    {
        isDying = true;
        animator.SetTrigger("Die");
        Debug.Log("Die trigger set");

        // Optionally, disable the slime enemy's ability to move or interact
        GetComponent<Collider2D>().enabled = false; // Disable collisions
        this.enabled = false; // Disable this script

        // Wait for a short moment to ensure the state transitions
        yield return new WaitForSeconds(0.1f);

        // Get the current animation length
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Death animation length: " + animationLength);

        // Wait for the death animation to complete before destroying the game object
        yield return new WaitForSeconds(animationLength);

        Destroy(gameObject);
    }
}
