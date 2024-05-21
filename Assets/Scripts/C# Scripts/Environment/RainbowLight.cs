using UnityEngine;
// Make sure you have the Universal Render Pipeline

public class RainbowLight : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light2D; // Reference to the 2D light component
    public float duration = 3.0f; // Duration of the full rainbow cycle

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        }
    }

    void Update()
    {
        // Calculate the hue value based on time
        float hue = Mathf.PingPong(Time.time / duration, 1);
        // Convert hue to RGB
        Color color = Color.HSVToRGB(hue, 1, 1);
        // Apply the color to the light
        light2D.color = color;
    }
}
