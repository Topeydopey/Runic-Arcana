using UnityEngine;

public class ObjectCollector2D : MonoBehaviour
{
    public GameObject[] targetObjects;  // Array of objects to be activated
    public string objectTag = "PickupObject";  // Tag of the objects to be picked up

    private int objectsCollected = 0;  // Counter for collected objects

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(objectTag))
        {
            // Activate the next target object if there are any left to activate
            if (objectsCollected < targetObjects.Length)
            {
                targetObjects[objectsCollected].SetActive(true);
                objectsCollected++;
            }

            // Optionally, you can destroy the picked-up object
            Destroy(other.gameObject);
        }
    }
}
