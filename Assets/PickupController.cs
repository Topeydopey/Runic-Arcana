using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform holdPoint;  // The point where objects are held
    public float pickUpDistance = 2f;  // How far the player can pick up objects
    private GameObject heldObject;
    private Rigidbody2D heldObjectRb;  // To store the Rigidbody of the held object

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                int layerMask = LayerMask.GetMask("Pickup"); // Only include the Pickup layer
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, pickUpDistance, layerMask);
                if (hit.collider != null)
                {
                    heldObject = hit.collider.gameObject;
                    heldObjectRb = heldObject.GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component

                    if (heldObjectRb != null)
                    {
                        heldObjectRb.isKinematic = true; // Set the Rigidbody to Kinematic
                    }

                    heldObject.transform.position = holdPoint.position;
                    heldObject.transform.parent = holdPoint;
                }
            }
            else
            {
                if (heldObjectRb != null)
                {
                    heldObjectRb.isKinematic = false; // Restore the Rigidbody to Dynamic
                }

                heldObject.transform.parent = null;
                heldObject = null;
                heldObjectRb = null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the ray in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * pickUpDistance);
    }
}
