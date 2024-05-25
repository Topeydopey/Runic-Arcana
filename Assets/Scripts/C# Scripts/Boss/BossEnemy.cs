using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class BossEnemy : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject[] slimePrefabs;
    public Transform[] fireballSpawnPoints;
    public float fireballCooldown = 1f;
    public float summonCooldown = 10f;
    public int maxHealth = 30;
    public int minionsToDefeat = 5;
    public AudioClip castingSound;
    public AudioClip[] hitSounds;
    public AudioClip deathSound;
    public TextMeshProUGUI bossHealthText;
    public CanvasGroup fadeCanvasGroup;
    public string endCreditSceneName = "EndCredits";
    public float fadeDuration = 2f;
    public AudioSource backgroundMusic; // Reference to the background music audio source

    private int currentHealth;
    private Transform player;
    private Animator animator;
    private bool isCasting = false;
    private bool isDying = false;
    private AudioSource audioSource;
    private int minionsDefeated = 0;
    private bool isActive = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;

        // Initialize the boss health text
        if (bossHealthText != null)
        {
            bossHealthText.text = "Demon Sorcerer Health: " + currentHealth;
        }

        // Ensure the fade canvas group is hidden initially
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
        }

        // Start the boss activation coroutine
        StartCoroutine(ActivateBossAfterDelay(3f)); // 3 seconds delay
    }

    void Update()
    {
        if (isDying || !isActive) return;

        // Check if all minions are defeated to trigger bullet hell mode
        if (minionsDefeated >= minionsToDefeat)
        {
            StopAllCoroutines();
            StartCoroutine(BulletHell());
        }
    }

    private IEnumerator ActivateBossAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateBoss();
    }

    public void ActivateBoss()
    {
        Debug.Log("Boss is activated."); // Debug statement
        isActive = true;
        StartCoroutine(AttackCycle());
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
            int slimeIndex = Random.Range(0, slimePrefabs.Length);
            Instantiate(slimePrefabs[slimeIndex], randomPosition, Quaternion.identity);
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

    private void PlayHitSound()
    {
        if (hitSounds.Length > 0)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            PlaySound(clip);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        PlayHitSound(); // Play a hit sound

        if (bossHealthText != null)
        {
            bossHealthText.text = "Demon Sorcerer Health: " + currentHealth; // Update boss health text
        }

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

        StartCoroutine(FadeMusic());
        // Fade to white and transition to end credit scene
        StartCoroutine(FadeToWhiteAndEndCredits());
    }

    private IEnumerator FadeToWhiteAndEndCredits()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(3f); // Wait for 3 seconds before transitioning to end credits

        SceneManager.LoadScene(endCreditSceneName); // Load end credit scene
    }

    private IEnumerator FadeMusic()
    {
        float startVolume = backgroundMusic.volume;

        while (backgroundMusic.volume > 0)
        {
            backgroundMusic.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        backgroundMusic.Stop();
        backgroundMusic.volume = startVolume;
    }

    public void MinionDefeated()
    {
        minionsDefeated++;
    }
}
