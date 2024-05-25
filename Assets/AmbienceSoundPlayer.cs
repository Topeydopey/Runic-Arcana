using UnityEngine;
using System.Collections;

public class AmbienceSoundPlayer : MonoBehaviour
{
    public AudioClip[] ambienceSounds; // Array of ambience sound clips
    public float minTimeBetweenSounds = 5f; // Minimum time between playing sounds
    public float maxTimeBetweenSounds = 15f; // Maximum time between playing sounds
    private AudioSource audioSource; // Audio source component

    private void Start()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start the coroutine to play sounds randomly
        StartCoroutine(PlayRandomAmbienceSound());
    }

    private IEnumerator PlayRandomAmbienceSound()
    {
        while (true)
        {
            // Play a random sound from the array
            if (ambienceSounds.Length > 0)
            {
                AudioClip randomSound = ambienceSounds[Random.Range(0, ambienceSounds.Length)];
                audioSource.clip = randomSound;
                audioSource.Play();

                // Wait for the sound to finish playing
                yield return new WaitForSeconds(randomSound.length);

                // Wait for a random time between the specified minimum and maximum before playing the next sound
                float waitTime = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
