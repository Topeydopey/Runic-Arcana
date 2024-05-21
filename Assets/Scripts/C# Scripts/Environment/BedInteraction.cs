using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this line to use UI elements
using System.Collections;

public class BedInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    public TextMeshProUGUI interactionPrompt; // Reference to the TMP Text element
    public float fadeDuration = 0.5f; // Duration for fade in/out

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Ensure the prompt is hidden at the start
        if (interactionPrompt != null)
        {
            canvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = interactionPrompt.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0; // Start hidden
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // Get the last completed level index
        int lastLevelCompleted = PlayerPrefs.GetInt("LastLevelCompleted", 0);
        // Load the next level by incrementing the index
        SceneManager.LoadScene(lastLevelCompleted + 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Show and fade in the interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
                StartCoroutine(FadeIn());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Fade out and hide the interaction prompt
            if (interactionPrompt != null)
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            yield return null;
        }
        interactionPrompt.gameObject.SetActive(false);
    }
}
