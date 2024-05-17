using UnityEngine;

public class CursorRotationAim : MonoBehaviour
{
    public GameObject fireballPrefab; // Assign this in the inspector
    public GameObject projectileSpawnPoint; // Assign the red box GameObject here in the inspector
    private Camera mainCam;
    private Vector3 direction; // Store direction

    void Start()
    {
        mainCam = Camera.main; // Simplified access to the main camera
    }

    void Update()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.nearClipPlane));
        mousePos.z = 0; // Set z to 0 for 2D

        direction = (mousePos - transform.position).normalized; // Direction from player to mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle); // Rotate the player or the aiming component
    }

    public Vector2 GetCurrentAimDirection()
    {
        return direction;
    }

    public float GetCurrentAimAngle()
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
