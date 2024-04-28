using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    // Call this method with the gesture ID when a gesture is recognized.
    public void CastSpell(string spellId)
    {
        switch (spellId)
        {
            case "horizontal":
                CastFireball();
                break;
                // ... Add other cases for different spellIds if necessary.
        }
    }

    // This method is called when a "Fireball" gesture is recognized.
    private void CastFireball()
    {
        // This is where you would implement the actual spell effect.
        // For testing purposes, we'll just print a message to the console.
        Debug.Log("Fireball spell cast!");
    }
}
