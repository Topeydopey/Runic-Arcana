using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 2;  // Player starts with 2 health points

    public void TakeDamage(int damage)
    {
        health -= damage;  // Reduce health by the damage amount
        if (health <= 0)
        {
            Die();  // Player dies if health reaches zero or less
        }
    }

    void Die()
    {
        Debug.Log("Player is Dead!");
        // Here you can add more logic for what happens when the player dies,
        // like playing a death animation, restarting the level, etc.
        gameObject.SetActive(false);  // Temporarily just disable the player object
    }
}
