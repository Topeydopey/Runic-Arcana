using UnityEngine;
using System.Collections;

public class TeleportingSlimeEnemy : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 2.0f;
    public float teleportRange = 7.0f;
    public float moveSpeed = 1.0f;
    public int maxHealth = 2;
    public GameObject teleportParticlePrefab;
    private int currentHealth;
    private Transform player;
    private Vector2 randomDirection;
    private float moveInterval = 2.0f;
    private bool isCharging = false;
    private Animator animator;
    private bool facingRight = true;
    public AudioClip damageSound;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float damageCooldown = 1.0f;
    private float lastDamageTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        InvokeRepeating("ChangeDirection", 0, moveInterval);
        audioSource = GetComponent<AudioSource>();
        lastDamageTime = -damageCooldown;
    }

    private void Update()
    {
        if (!isCharging)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRange)
            {
                StartCoroutine(TeleportAndCharge());
            }
            else
            {
                MoveRandomly();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            Debug.Log("Hit Fireball");
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
    }

    private void ApplyDamage(GameObject player)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1);
            lastDamageTime = Time.time;
            Debug.Log("Player damaged by teleporting slime");
        }
    }

    private void ChangeDirection()
    {
        randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        FlipSprite(randomDirection.x);
    }

    private void MoveRandomly()
    {
        animator.SetBool("IsMoving", true);
        rb.MovePosition(rb.position + randomDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator TeleportAndCharge()
    {
        isCharging = true;
        animator.SetBool("IsCharging", true);

        // Teleport to a position within teleportRange of the player
        Vector2 teleportPosition = (Vector2)player.position + (Random.insideUnitCircle.normalized * teleportRange);

        // Instantiate the teleport particle system at the current position before teleporting
        Instantiate(teleportParticlePrefab, transform.position, Quaternion.identity);

        // Teleport to the new position
        transform.position = teleportPosition;

        // Instantiate the teleport particle system at the new position after teleporting
        Instantiate(teleportParticlePrefab, teleportPosition, Quaternion.identity);

        // Stop for a moment before charging
        yield return new WaitForSeconds(0.5f);

        Vector2 chargeDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        FlipSprite(chargeDirection.x);

        float chargeDuration = 1.0f;
        float elapsedTime = 0;

        while (elapsedTime < chargeDuration)
        {
            rb.MovePosition(rb.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop for 1 second after charging
        yield return new WaitForSeconds(1.0f);

        animator.SetBool("IsCharging", false);
        isCharging = false;
    }

    private void FlipSprite(float direction)
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
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");

        PlayDamageSound();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private void PlayDamageSound()
    {
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }
}
