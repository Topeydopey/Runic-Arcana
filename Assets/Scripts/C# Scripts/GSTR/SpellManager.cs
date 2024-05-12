using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public CursorRotationAim cursorRotationAim; // Assign this in the inspector
    public GameObject barrierPrefab; // Assign the barrier prefab in the inspector
    public Transform playerTransform; // Assign the player's transform here in the inspector
    private GameObject barrierInstance; // Holds the instantiated barrier

    void Start()
    {
        // Check if the player's transform is assigned
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the SpellManager.");
            return;
        }

        // Instantiate the barrier at the player's current position and keep it deactivated
        if (barrierPrefab != null)
        {
            barrierInstance = Instantiate(barrierPrefab, playerTransform.position, Quaternion.identity);
            barrierInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("Barrier prefab not assigned.");
        }
    }

    public void CastSpell(string spellId)
    {
        switch (spellId)
        {
            case "kenaz":
                CastFireball();
                break;
            case "uruz":
                ToggleBarrier();
                break;
        }
    }

    private void CastFireball()
    {
        if (cursorRotationAim.fireballPrefab != null)
        {
            Vector2 direction = cursorRotationAim.GetCurrentAimDirection();
            float angle = cursorRotationAim.GetCurrentAimAngle();
            GameObject fireball = Instantiate(cursorRotationAim.fireballPrefab, cursorRotationAim.projectileSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle)));
            if (fireball.GetComponent<Fireball>() != null)
            {
                fireball.GetComponent<Fireball>().Initialize(direction);
            }
            else
            {
                Debug.LogError("Fireball script not found on the instantiated prefab!");
            }
        }
        else
        {
            Debug.LogError("Fireball prefab not assigned.");
        }
    }

    private void ToggleBarrier()
    {
        if (barrierInstance != null)
        {
            barrierInstance.SetActive(!barrierInstance.activeSelf);
        }
        else
        {
            Debug.LogError("Barrier instance not found.");
        }
    }

    void Update()
    {
        if (barrierInstance != null && barrierInstance.activeSelf)
        {
            // Update the barrier's position to be at the player's position
            barrierInstance.transform.position = playerTransform.position;

            // Update the rotation to face towards the cursor
            Vector2 direction = cursorRotationAim.GetCurrentAimDirection();
            float angle = cursorRotationAim.GetCurrentAimAngle();
            barrierInstance.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }

        if (Input.GetKeyDown(KeyCode.B)) // Press B to cast barrier
        {
            CastSpell("uruz");
        }
    }
}
