using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Include this to manage scene transitions
using System.Collections;

public class EventTrigger : MonoBehaviour
{
    public GameObject[] objectsToInstantiate; // Array of objects to instantiate
    public float radius = 5f; // Radius around the player to instantiate objects
    public int numberOfObjects = 5; // Number of objects to instantiate
    public AudioClip eventSound; // Sound to play during the event
    public float fadeDuration = 1f; // Duration for fade to white
    public CanvasGroup fadeCanvasGroup; // Canvas group for the fade effect
    public string nextLevelName; // Name of the next level to load

    private AudioSource audioSource; // Audio source component
    private bool hasBeenTriggered = false; // Flag to ensure the event only happens once
    private GameObject player; // Reference to the player

    private void Start()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure the fade canvas group is initially hidden
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            hasBeenTriggered = true; // Set the flag to prevent re-triggering
            player = other.gameObject;
            // Start the event sequence
            StartCoroutine(EventSequence());
        }
    }

    private IEnumerator EventSequence()
    {
        // Stop player movement
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Play the event sound
        if (eventSound != null)
        {
            audioSource.PlayOneShot(eventSound);
        }

        // Instantiate objects around the player
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector2 randomPosition = (Vector2)player.transform.position + Random.insideUnitCircle * radius;
            Instantiate(objectsToInstantiate[Random.Range(0, objectsToInstantiate.Length)], randomPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.1f); // Optional delay between instantiations
        }

        // Fade to white
        yield return StartCoroutine(FadeToWhite(fadeDuration));

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Load the next level
        SceneManager.LoadScene(nextLevelName);
    }

    private IEnumerator FadeToWhite(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            }
            yield return null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
        }
    }
}
