using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Animator chestAnimator;
    public string devourTriggerName = "DevourTrigger";
    public float interactionRange = 2f;
    public Transform chestTransform;
    public GameObject player; // Reference to the player GameObject
    public FadeController fadeController; // Reference to the FadeController script
    public AudioClip devourSound; // Audio clip for the devour sound
    private AudioSource audioSource; // Audio source component
    public string nextSceneName = "NextLevel"; // Name of the next scene to load

    private void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceToChest = Vector3.Distance(player.transform.position, chestTransform.position);
            if (distanceToChest <= interactionRange)
            {
                // Trigger the devour animation
                chestAnimator.SetTrigger(devourTriggerName);

                // Play the devour sound
                if (devourSound != null)
                {
                    audioSource.PlayOneShot(devourSound);
                }

                // Start the coroutine to disable the player and transition to the next level
                StartCoroutine(DevourPlayerAndTransition());
            }
        }
    }

    private IEnumerator DevourPlayerAndTransition()
    {
        // Disable the player GameObject
        player.SetActive(false);

        // Start the fade to black
        fadeController.FadeToScene(nextSceneName);

        // Wait for the fade duration
        yield return new WaitForSeconds(fadeController.fadeDuration);

        // Wait for the length of the audio clip or an additional 8 seconds, whichever is longer
        yield return new WaitForSeconds(Mathf.Max(8f, devourSound.length));

        // Load the specified scene
        SceneManager.LoadScene(nextSceneName);
    }
}
