using UnityEngine;
using System.Collections;

public class FireSlimeEnemy : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float moveSpeed = 1.0f;
    public int maxHealth = 2;  // Maximum health
    private int currentHealth;
    private Transform player;
    private Vector2 randomDirection;
    private float moveInterval = 2.0f; // Interval in seconds between direction changes
    private Animator animator;
    private bool facingRight = true; // Assuming the initial facing direction is right
    private bool isDying = false; // Flag to check if the slime is dying
    private Rigidbody2D rb;

    public GameObject projectilePrefab; // Projectile to be shot
    public float projectileSpeed = 10f; // Speed of the projectile
    public float shootCooldown = 2.0f; // Cooldown period for shooting

    private float lastShootTime; // Timestamp of the last shot

    private float damageCooldown = 1.0f; // Cooldown period between damage applications
    private float lastDamageTime; // Timestamp of the last damage application

    public AudioClip damageSound; // Audio clip for damage sound
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
        lastShootTime = -shootCooldown; // Initialize to allow immediate shooting

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        Debug.Log("FireSlimeEnemy initialized with maxHealth: " + maxHealth);
    }

    void Update()
    {
        if (isDying) return; // If dying, don't perform any other actions

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                ShootAtPlayer();
                lastShootTime = Time.time; // Update the last shoot time
                Debug.Log("Shooting at player");
                animator.SetBool("IsMoving", false); // Stop moving animation when shooting
            }
            else
            {
                Debug.Log("Waiting for cooldown");
            }
        }
        else
        {
            MoveRandomly();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name); // Debugging statement
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            Debug.Log("Hit by Fireball"); // Debugging statement
            TakeDamage(1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            other.GetComponent<PlayerHealth>().TakeDamage(1);
        }
        else if (other.CompareTag("Fireball"))
        {
            Debug.Log("Hit Fireball");
            TakeDamage(1);
        }
        else if (other.CompareTag("Spell"))
        {
            Debug.Log("Hit by Spell"); // Debugging statement
            TakeDamage(1);
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
        Vector2 newPosition = rb.position + randomDirection * moveSpeed * Time.fixedDeltaTime;
        if (newPosition != rb.position)
        {
            animator.SetBool("IsMoving", true); // Set moving animation when changing position
        }
        else
        {
            animator.SetBool("IsMoving", false); // Stop moving animation if not changing position
        }
        rb.MovePosition(newPosition);
    }

    void ShootAtPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(direction.x);

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        SlimeProjectile slimeProjectile = projectile.GetComponent<SlimeProjectile>();
        if (slimeProjectile != null)
        {
            slimeProjectile.Initialize(direction);
        }
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
        Debug.Log("Attempting to take damage: " + damage); // Debugging statement
        if (isDying)
        {
            Debug.Log("Already dying, no damage taken."); // Debugging statement
            return; // If dying, don't take any more damage
        }

        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        Debug.Log("FireSlime took damage: " + damage + ", currentHealth: " + currentHealth); // Debugging statement

        PlayDamageSound(); // Play damage sound

        if (currentHealth <= 0)
        {
            Debug.Log("Current health is zero or less, starting death coroutine."); // Debugging statement
            StartCoroutine(Die());
        }
    }

    void PlayDamageSound()
    {
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    IEnumerator Die()
    {
        isDying = true;
        animator.SetTrigger("Die");
        Debug.Log("Die trigger set");

        // Optionally, disable the slime enemy's ability to move or interact
        GetComponent<Collider2D>().enabled = false; // Disable collisions
        rb.velocity = Vector2.zero; // Stop movement
        this.enabled = false; // Disable this script

        // Wait for 1 second before destroying the game object
        yield return new WaitForSeconds(1.0f);

        Debug.Log("Destroying FireSlimeEnemy object.");
        Destroy(gameObject);
    }
}
