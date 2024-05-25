using UnityEngine;

public class CampfireAudio : MonoBehaviour
{
    public AudioClip fireSound; // The fire audio clip
    private AudioSource audioSource; // The AudioSource component

    void Start()
    {
        // Get the AudioSource component attached to the campfire
        audioSource = GetComponent<AudioSource>();

        // Check if the AudioSource component is present
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on the campfire GameObject.");
            return;
        }

        // Set the audio clip to the AudioSource component
        audioSource.clip = fireSound;

        // Enable looping
        audioSource.loop = true;

        // Play the audio
        audioSource.Play();
    }
}
