using GestureRecognizer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public GameObject drawUI;
    public DrawDetector drawDetector;
    public Volume slowMoVolume; // Ensure this is a URP Volume component

    private bool isUIActive = false;
    private ColorAdjustments colorAdjustments = null;

    void Start()
    {
        // Try to fetch the Color Adjustments component from the volume
        if (slowMoVolume.profile.TryGet(out colorAdjustments))
        {
            // Optionally, disable the effect initially
            colorAdjustments.active = false;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (!isUIActive)
            {
                ActivateSlowMo(true);
            }
        }
        else
        {
            if (isUIActive)
            {
                ActivateSlowMo(false);
            }
        }
    }

    void ActivateSlowMo(bool activate)
    {
        Time.timeScale = activate ? 0.3f : 1.0f;
        isUIActive = activate;
        drawUI.SetActive(activate);
        if (drawDetector != null && !activate)
        {
            drawDetector.ResetDrawingState();
        }

        // Adjust color adjustments when slow-mo is activated
        if (colorAdjustments != null)
        {
            colorAdjustments.active = activate;
            colorAdjustments.saturation.value = activate ? -50 : 0;  // Desaturate when slow-mo is active
        }
    }
}

