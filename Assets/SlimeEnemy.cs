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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        InvokeRepeating("ChangeDirection", 0, moveInterval);
    }

    void Update()
    {
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
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
        }
        else if (collision.gameObject.CompareTag("Fireball"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the fireball upon collision
        }
    }

    void ChangeDirection()
    {
        randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void MoveRandomly()
    {
        transform.Translate(randomDirection * moveSpeed * Time.deltaTime);
    }

    IEnumerator LeapAtPlayer()
    {
        isLeaping = true;
        animator.SetBool("IsLeaping", true);

        // Stop for a moment before leaping
        yield return new WaitForSeconds(0.5f);

        Vector3 leapDirection = (player.position - transform.position).normalized;

        float leapDuration = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < leapDuration)
        {
            transform.Translate(leapDirection * leapSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop for 1 second after leaping
        yield return new WaitForSeconds(1.0f);

        animator.SetBool("IsLeaping", false);
        isLeaping = false;
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
