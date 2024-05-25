using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    // Reference to the FadeController script
    public FadeController fadeController;

    // Reference to the TextMeshProUGUI component for the prompt
    public TextMeshProUGUI promptText;
    // Audio clip for the enter sound
    public AudioClip enterSound;
    private AudioSource audioSource;

    private bool isPlayerNear = false; // Flag to check if the player is near
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Ensure the prompt text is initially hidden
        SetTextAlpha(0);
        promptText.gameObject.SetActive(true);
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Check if the player is near and the "E" key is pressed
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Fade in the prompt when the player is near
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeText(1f, 0.5f));
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Fade out the prompt when the player leaves
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeText(0f, 0.5f));
            isPlayerNear = false;
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Play the enter sound
        if (enterSound != null)
        {
            audioSource.PlayOneShot(enterSound);
        }

        // Start the fade to scene transition
        fadeController.FadeToScene("House");

        // Wait for the fade duration before proceeding
        yield return new WaitForSeconds(fadeController.fadeDuration);

        // Optional: Wait for the sound to finish playing before fully transitioning
        if (enterSound != null)
        {
            yield return new WaitForSeconds(enterSound.length - fadeController.fadeDuration);
        }
    }

    private IEnumerator FadeText(float targetAlpha, float duration)
    {
        float startAlpha = promptText.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            promptText.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        promptText.alpha = targetAlpha;
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = promptText.color;
        color.a = alpha;
        promptText.color = color;
    }
}
