using UnityEngine;
using TMPro;
using System.Collections;

public class BossNarration : MonoBehaviour
{
    public TextMeshProUGUI narrationText; // Reference to the TMP Text element for narration
    public string message; // The message to display
    public float fadeDuration = 1f; // Duration for fade in/out
    public float displayDuration = 3f; // Duration to display the message
    public AudioClip narrationSound; // The sound to play
    private AudioSource audioSource; // Audio source component
    public Color narrationColor = Color.red; // Color of the narration text

    private CanvasGroup canvasGroup; // Canvas group to handle fading
    private bool hasBeenTriggered = false; // Flag to ensure the narration only appears once

    private void Start()
    {
        // Ensure the text is initially hidden
        canvasGroup = narrationText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = narrationText.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; // Start hidden

        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            hasBeenTriggered = true; // Set the flag to prevent re-triggering
            // Play the narration sound
            if (narrationSound != null)
            {
                audioSource.PlayOneShot(narrationSound);
            }
            // Start the narration sequence
            StartCoroutine(DisplayNarration());
        }
    }

    private IEnumerator DisplayNarration()
    {
        // Set the narration text and color
        narrationText.text = message;
        narrationText.color = narrationColor;

        // Fade in
        yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));

        // Display the text for the set duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(FadeText(1f, 0f, fadeDuration));
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
