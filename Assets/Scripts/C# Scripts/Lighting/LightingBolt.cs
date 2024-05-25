using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class LightningBolt : MonoBehaviour
{
    public float lifetime = 0.14f; // Total duration for which the lightning bolt will be visible
    public int damage = 1; // Damage the lightning bolt will deal to enemies
    public Light2D lightningLight; // Reference to the Light2D component
    public float maxIntensity = 1f; // Maximum intensity of the light

    private float halfLifetime; // Half of the total lifetime to manage fade in and out
    private float timer = 0f; // Timer to track the elapsed time

    void Start()
    {
        if (lightningLight == null)
        {
            Debug.LogError("Light2D component not assigned.");
        }

        lightningLight.intensity = 0f; // Start with light intensity at 0
        halfLifetime = lifetime / 2f;
        Destroy(gameObject, lifetime); // Destroy the lightning bolt after its lifetime
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (lightningLight != null)
        {
            if (timer < halfLifetime)
            {
                // Fade in
                float intensity = Mathf.Lerp(0f, maxIntensity, timer / halfLifetime);
                lightningLight.intensity = intensity;
            }
            else
            {
                // Fade out
                float intensity = Mathf.Lerp(maxIntensity, 0f, (timer - halfLifetime) / halfLifetime);
                lightningLight.intensity = intensity;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Damage the enemy
            SlimeEnemy slimeEnemy = collision.GetComponent<SlimeEnemy>();
            FireSlimeEnemy fireSlimeEnemy = collision.GetComponent<FireSlimeEnemy>();
            TeleportingSlimeEnemy teleportingSlimeEnemy = collision.GetComponent<TeleportingSlimeEnemy>(); // Check for TeleportingSlimeEnemy

            if (slimeEnemy != null)
            {
                slimeEnemy.TakeDamage(damage);
            }
            else if (fireSlimeEnemy != null)
            {
                fireSlimeEnemy.TakeDamage(damage);
            }
            else if (teleportingSlimeEnemy != null) // Apply damage to TeleportingSlimeEnemy
            {
                teleportingSlimeEnemy.TakeDamage(damage);
            }
        }
    }
}
