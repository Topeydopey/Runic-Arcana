using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    public GameObject fireballPrefab; // Reference to the fireball projectile prefab
    public GameObject slimePrefab; // Reference to the slime enemy prefab
    public Transform[] fireballSpawnPoints; // Array of points to spawn fireballs from
    public float fireballCooldown = 1f; // Cooldown between fireball attacks
    public float summonCooldown = 10f; // Cooldown between summoning minions
    public int maxHealth = 30; // Boss health
    public int minionsToDefeat = 5; // Number of minions the player needs to defeat
    public AudioClip castingSound; // Sound for casting spells
    public AudioClip hitSound; // Sound for when the boss is hit
    public AudioClip deathSound; // Sound for dying

    private int currentHealth;
    private Transform player;
    private Animator animator;
    private bool isCasting = false;
    private bool isDying = false;
    private AudioSource audioSource;
    private int minionsDefeated = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        StartCoroutine(AttackCycle());
    }

    void Update()
    {
        if (isDying) return;

        // Check if all minions are defeated to trigger bullet hell mode
        if (minionsDefeated >= minionsToDefeat)
        {
            StopAllCoroutines();
            StartCoroutine(BulletHell());
        }
    }

    private IEnumerator AttackCycle()
    {
        while (!isDying)
        {
            yield return StartCoroutine(CastFireballs());
            yield return new WaitForSeconds(fireballCooldown);
            yield return StartCoroutine(SummonMinions());
            yield return new WaitForSeconds(summonCooldown);
        }
    }

    private IEnumerator CastFireballs()
    {
        if (isDying) yield break;

        isCasting = true;
        animator.SetTrigger("Cast");
        PlaySound(castingSound);

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation

        foreach (Transform spawnPoint in fireballSpawnPoints)
        {
            Vector2 direction = (player.position - spawnPoint.position).normalized;
            GameObject projectile = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);
            SlimeProjectile slimeProjectile = projectile.GetComponent<SlimeProjectile>();
            if (slimeProjectile != null)
            {
                slimeProjectile.Initialize(direction);
            }
        }

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation
        isCasting = false;
        animator.SetTrigger("Idle"); // Return to idle animation
    }

    private IEnumerator SummonMinions()
    {
        if (isDying) yield break;

        isCasting = true;
        animator.SetTrigger("Cast");
        PlaySound(castingSound);

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation

        for (int i = 0; i < minionsToDefeat; i++)
        {
            Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * 3f;
            Instantiate(slimePrefab, randomPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation
        isCasting = false;
        animator.SetTrigger("Idle"); // Return to idle animation
    }

    private IEnumerator BulletHell()
    {
        if (isDying) yield break;

        isCasting = true;
        animator.SetTrigger("Cast");
        PlaySound(castingSound);

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation

        // Implement bullet hell attack logic here

        yield return new WaitForSeconds(0.5f); // Delay to sync with casting animation
        isCasting = false;
        animator.SetTrigger("Idle"); // Return to idle animation
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        PlaySound(hitSound); // Play hit sound

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDying = true;
        PlaySound(deathSound);

        yield return new WaitForSeconds(1f); // Delay for dying animation

        // Add any additional logic for when the boss dies

        Destroy(gameObject);
    }

    public void MinionDefeated()
    {
        minionsDefeated++;
    }
}
