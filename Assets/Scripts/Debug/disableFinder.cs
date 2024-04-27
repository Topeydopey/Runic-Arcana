using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class disableFinder : MonoBehaviour
{
    // ... other script code

    void OnDisable()
    {
        // This code will be called when the GameObject is disabled
        Debug.LogError("GameObject disabled!");  // Option 1: Log a message with callstack
    }
}
