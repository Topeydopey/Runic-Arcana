using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.SendMessage("ApplyStopEffect");
            Destroy(gameObject); // Destroy the fireball upon hitting the enemy
        }
    }
}
