using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;  // Player starts with 2 health points
    public TextMeshProUGUI healthDisplay;  // Reference to the TMP UI element

    void Start()
    {
        UpdateHealthDisplay();  // Update the display on start
    }

    public void TakeDamage(int damage)
    {
        health -= damage;  // Reduce health by the damage amount
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
        Debug.Log("Player is Dead!");
        gameObject.SetActive(false);  // Temporarily just disable the player object
    }
}
