using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float knockbackForce = 10f; // Adjust this value based on your game's needs

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Debug.Log("Enemy hit");
    }
}
