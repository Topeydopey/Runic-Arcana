using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;  // Player starts with 2 health points
    public TextMeshProUGUI healthDisplay;  // Reference to the TMP UI element
    public AudioClip[] damageSounds;  // Array of sounds played when the player takes damage
    public AudioClip deathSound;  // Sound played when the player dies
    private AudioSource audioSource;  // Reference to the AudioSource component
    private Animator animator;  // Reference to the Animator component
    private bool isDead = false;  // To track if the player is dead
    private Rigidbody2D rb;  // Reference to the Rigidbody2D component
    public ScreenFader screenFader;  // Reference to the ScreenFader script

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        UpdateHealthDisplay();  // Update the display on start
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;  // If the player is already dead, ignore further damage

        health -= damage;  // Reduce health by the damage amount
        animator.SetTrigger("Hit");  // Trigger the "Hit" animation
        PlayRandomDamageSound();  // Play a random damage sound
        UpdateHealthDisplay();  // Update the UI whenever health changes

        if (health <= 0)
        {
            Die();  // Player dies if health reaches zero or less
        }
    }

    void UpdateHealthDisplay()
    {
        if (healthDisplay != null)
            healthDisplay.text = "Health: " + health;  // Update the text to show current health
        else
            Debug.LogWarning("Health display reference not set on PlayerHealth.");
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player is Dead!");
        animator.SetTrigger("Die");  // Trigger the "Die" animation
        PlaySound(deathSound);  // Play death sound
        // Disable player movement and other interactions
        GetComponent<PlayerMovement>().enabled = false;  // Assuming you have a PlayerMovement script
        // Optionally disable other components or add more death logic here
        rb.constraints = RigidbodyConstraints2D.FreezeAll;  // Freeze all movement and rotation
        screenFader.FadeToBlackAndRestart();  // Call the fade out and restart method
    }

    private void PlayRandomDamageSound()
    {
        if (damageSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
