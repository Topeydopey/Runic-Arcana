using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    // Assign the GameObject you want to disable in the Inspector
    public GameObject targetGameObject;

    void Update()
    {
        // Check if the "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Toggle the active state of the target GameObject
            targetGameObject.SetActive(!targetGameObject.activeSelf);
        }
    }
    ///// ARG I AHTE NIGGERRS SO BADARAGG ARGGG MWHY IEM&*ULATOR  NIGGWET  {ISNT WF I)SDNfklmsdj fjkdnsjofnsdojfbsdkhfj edskwhff hdik ughjkb}
}
