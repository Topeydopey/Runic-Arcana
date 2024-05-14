using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 2.0f;
    public float moveSpeed = 1.0f;
    private Transform player;
    private Vector2 randomDirection;
    private float moveInterval = 2.0f; // Interval in seconds between direction changes

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("ChangeDirection", 0, moveInterval);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            ChargePlayer();
        }
        else
        {
            MoveRandomly();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
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

    void ChargePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chargeSpeed * Time.deltaTime);
    }
}
