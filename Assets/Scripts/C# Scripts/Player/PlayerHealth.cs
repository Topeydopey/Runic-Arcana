using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;  // Player starts with 2 health points
    public TextMeshProUGUI healthDisplay;  // Reference to the TMP UI element
    private Animator animator;  // Reference to the Animator component
    private bool isDead = false;  // To track if the player is dead

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
        UpdateHealthDisplay();  // Update the display on start
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;  // If the player is already dead, ignore further damage

        health -= damage;  // Reduce health by the damage amount
        animator.SetTrigger("Hit");  // Trigger the "Hit" animation
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
        // Disable player movement and other interactions
        GetComponent<PlayerMovement>().enabled = false;  // Assuming you have a PlayerMovement script
        // Optionally disable other components or add more death logic here
    }
}
