using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI thankYouText;
    public float fadeDuration = 2.0f;
    public float displayDuration = 2.0f;

    void Start()
    {
        creditsText.alpha = 0.0f;
        thankYouText.alpha = 0.0f;
        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // Fade in the credits text
        yield return StartCoroutine(FadeTextToFullAlpha(fadeDuration, creditsText));
        yield return new WaitForSeconds(displayDuration);

        // Fade out the credits text
        yield return StartCoroutine(FadeTextToZeroAlpha(fadeDuration, creditsText));
        yield return new WaitForSeconds(fadeDuration);

        // Fade in the thank you text
        yield return StartCoroutine(FadeTextToFullAlpha(fadeDuration, thankYouText));
        yield return new WaitForSeconds(displayDuration);

        // Fade out the thank you text
        yield return StartCoroutine(FadeTextToZeroAlpha(fadeDuration, thankYouText));
        yield return new WaitForSeconds(fadeDuration);

        // Transition to Main Menu
        SceneManager.LoadScene("Main Menu");
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.alpha < 1.0f)
        {
            text.alpha += Time.deltaTime / t;
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.alpha > 0.0f)
        {
            text.alpha -= Time.deltaTime / t;
            yield return null;
        }
    }
}
