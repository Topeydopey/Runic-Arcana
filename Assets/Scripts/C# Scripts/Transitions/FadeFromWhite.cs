using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeFromWhite : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // Canvas group for fading effect
    public float fadeDuration = 2f; // Duration of the fade effect
    public string sceneToLoad; // Name of the scene to load after fading

    void Start()
    {
        StartCoroutine(FadeInAndLoadScene());
    }

    private IEnumerator FadeInAndLoadScene()
    {
        float elapsedTime = 0f;

        // Start with the canvas group alpha set to 1 (fully white)
        fadeCanvasGroup.alpha = 1f;

        // Fade from white to transparent
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        // Load the specified scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
