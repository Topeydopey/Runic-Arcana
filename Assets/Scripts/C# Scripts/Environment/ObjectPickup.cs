using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectCollector2D : MonoBehaviour
{
    public GameObject[] targetObjects;  // Array of objects to be activated
    public string objectTag = "PickupObject";  // Tag of the objects to be picked up

    public TextMeshProUGUI messageText;  // Reference to the TextMeshProUGUI component
    public CanvasGroup canvasGroup;  // Reference to the CanvasGroup component for fading

    private int objectsCollected = 0;  // Counter for collected objects

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(objectTag))
        {
            // Activate the next target object if there are any left to activate
            if (objectsCollected < targetObjects.Length)
            {
                targetObjects[objectsCollected].SetActive(true);
                objectsCollected++;
            }

            // Show the "Spell Collected" message
            StartCoroutine(ShowMessage("Spell Collected", 2f));

            // Optionally, you can destroy the picked-up object
            Destroy(other.gameObject);
        }
    }

    private IEnumerator ShowMessage(string message, float duration)
    {
        messageText.text = message;
        yield return FadeCanvasGroup(0f, 1f, 0.5f);  // Fade in
        yield return new WaitForSeconds(duration);   // Wait for the specified duration
        yield return FadeCanvasGroup(1f, 0f, 0.5f);  // Fade out
    }

    private IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}
