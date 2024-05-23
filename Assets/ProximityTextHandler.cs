using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class ProximityTextHandler : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float fadeDuration = 1.0f;
    public CircleCollider2D proximityCollider;
    public Transform playerTransform;

    private bool isPlayerNearby = false;
    private float fadeVelocity = 0.0f;

    void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro component is not assigned.");
            return;
        }

        if (proximityCollider == null)
        {
            Debug.LogError("Proximity Collider is not assigned.");
            return;
        }

        proximityCollider.isTrigger = true;

        // Initially set text to be invisible
        SetTextAlpha(0);
    }

    void Update()
    {
        // Check if the player is within the collider bounds
        if (proximityCollider.OverlapPoint(playerTransform.position))
        {
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
                StopAllCoroutines();
                StartCoroutine(FadeTextToFullAlpha());
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                StopAllCoroutines();
                StartCoroutine(FadeTextToZeroAlpha());
            }
        }
    }

    private IEnumerator FadeTextToFullAlpha()
    {
        float alpha = textMeshPro.color.a;
        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime / fadeDuration;
            SetTextAlpha(alpha);
            yield return null;
        }
        SetTextAlpha(1.0f);
    }

    private IEnumerator FadeTextToZeroAlpha()
    {
        float alpha = textMeshPro.color.a;
        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            SetTextAlpha(alpha);
            yield return null;
        }
        SetTextAlpha(0.0f);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = textMeshPro.color;
        color.a = alpha;
        textMeshPro.color = color;
    }
}
