using UnityEngine;
using TMPro;
using System.Collections;

public class BookshelfInteraction : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_Text promptText; // Text for the "Press E" prompt
    public float fadeDuration = 1f;
    private bool isPlayerNearby = false;
    private bool isTextVisible = false;
    private CanvasGroup messageCanvasGroup;
    private CanvasGroup promptCanvasGroup;

    void Start()
    {
        messageCanvasGroup = messageText.GetComponent<CanvasGroup>();
        if (messageCanvasGroup == null)
        {
            messageCanvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();
        }
        messageCanvasGroup.alpha = 0;
        messageText.gameObject.SetActive(false);

        promptCanvasGroup = promptText.GetComponent<CanvasGroup>();
        if (promptCanvasGroup == null)
        {
            promptCanvasGroup = promptText.gameObject.AddComponent<CanvasGroup>();
        }
        promptCanvasGroup.alpha = 0;
        promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTextVisible)
            {
                StartCoroutine(FadeOutText(promptCanvasGroup, promptText));
                StartCoroutine(FadeInText(messageCanvasGroup, messageText));
            }
            else
            {
                StartCoroutine(FadeOutText(messageCanvasGroup, messageText));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            promptText.gameObject.SetActive(true);
            StartCoroutine(FadeInText(promptCanvasGroup, promptText));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            StartCoroutine(FadeOutText(promptCanvasGroup, promptText));
            if (isTextVisible)
            {
                StartCoroutine(FadeOutText(messageCanvasGroup, messageText));
            }
        }
    }

    private IEnumerator FadeInText(CanvasGroup canvasGroup, TMP_Text textElement)
    {
        textElement.gameObject.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        if (textElement == messageText)
        {
            isTextVisible = true;
        }
    }

    private IEnumerator FadeOutText(CanvasGroup canvasGroup, TMP_Text textElement)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        textElement.gameObject.SetActive(false);
        if (textElement == messageText)
        {
            isTextVisible = false;
        }
    }
}
