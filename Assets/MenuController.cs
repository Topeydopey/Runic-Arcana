using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public string startButtonTag = "StartButton";
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for mouse click
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag(startButtonTag))
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("Level 1"); // Replace "NextLevel" with your scene name
    }

    IEnumerator FadeToBlack()
    {
        Color fadeColor = fadeImage.color;
        float alpha = fadeColor.a;

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            fadeColor.a = Mathf.Lerp(alpha, 1, normalizedTime);
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1;
        fadeImage.color = fadeColor;
    }
}
