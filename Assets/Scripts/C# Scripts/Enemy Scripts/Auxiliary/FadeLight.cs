using System.Collections;
using UnityEngine;


public class FadeLight : MonoBehaviour
{
    public float fadeDuration = 2.0f;
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private float initialIntensity;

    void Start()
    {
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found on this GameObject.");
            return;
        }
        initialIntensity = light2D.intensity;
        StartCoroutine(FadeOutLight());
    }

    IEnumerator FadeOutLight()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            light2D.intensity = Mathf.Lerp(initialIntensity, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        light2D.intensity = 0;
    }
}
