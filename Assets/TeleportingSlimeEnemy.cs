using UnityEngine;
using System.Collections;

public class TeleportingSlimeEnemy : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 2.0f;
    public float teleportRange = 7.0f;
    public float moveSpeed = 1.0f;
    public int maxHealth = 2;  // Maximum health
    public GameObject teleportParticlePrefab; // Reference to the particle system prefab
    private int currentHealth;
    private Transform player;
    private Vector2 randomDirection;
    private float moveInterval = 2.0f; // Interval in seconds between direction changes
    private bool isCharging = false;
    private Animator animator;
    private bool facingRight = true; // Assuming the initial facing direction is right

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        InvokeRepeating("ChangeDirection", 0, moveInterval);
    }

    void Update()
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
    }

    void ChangeDirection()
    {
        randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        FlipSprite(randomDirection.x);
    }

    void MoveRandomly()
    {
        animator.SetBool("IsMoving", true);
        transform.Translate(randomDirection * moveSpeed * Time.deltaTime);
    }

    IEnumerator TeleportAndCharge()
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
            transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop for 1 second after charging
        yield return new WaitForSeconds(1.0f);

        animator.SetBool("IsCharging", false);
        isCharging = false;
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
        // Optionally, disable the slime enemy's ability to move or interact
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false; // Disable this script

        // Wait for the death animation to complete before destroying the game object
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }
}
