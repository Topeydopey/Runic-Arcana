using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private void Start()
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1.0f;
        fadeImage.color = color;
        SceneManager.LoadScene(sceneName);
    }
}
