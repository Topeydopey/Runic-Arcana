using UnityEngine;

public class Barrier : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            Debug.Log("Enemy hit");
    }
}
