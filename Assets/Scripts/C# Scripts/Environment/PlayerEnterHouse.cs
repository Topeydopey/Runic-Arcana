using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    // Reference to the FadeController script
    public FadeController fadeController;

    // This method is called when the player collides with another collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Trigger the fade to scene transition
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        fadeController.FadeToScene("House");
        // Wait for the fade duration before proceeding
        yield return new WaitForSeconds(fadeController.fadeDuration);
    }
}
