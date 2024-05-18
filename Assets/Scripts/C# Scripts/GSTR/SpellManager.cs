using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public CursorRotationAim cursorRotationAim; // Assign this in the inspector
    public GameObject barrierPrefab; // Assign the barrier prefab in the inspector
    public Transform playerTransform; // Assign the player's transform here in the inspector
    private GameObject barrierInstance; // Holds the instantiated barrier

    // Add a reference to the player's Animator
    private Animator playerAnimator;
    private bool isCasting = false;
    private string queuedSpellId;

    void Start()
    {
        // Check if the player's transform is assigned
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the SpellManager.");
            return;
        }

        // Get the Animator component from the player
        playerAnimator = playerTransform.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator not found on the player.");
        }

        // Instantiate the barrier at the start but keep it deactivated
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
        if (isCasting)
        {
            // Queue the spell to cast after current cast is finished
            queuedSpellId = spellId;
            return;
        }

        switch (spellId)
        {
            case "kenaz":
                StartFireballCasting();
                break;
            case "uruz":
                ToggleBarrier();
                break;
        }
    }

    private void StartFireballCasting()
    {
        // Trigger the fireball animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Cast");
            isCasting = true; // Indicate that casting is in progress
        }
    }

    // This method will be called by the animation event at the end of the fireball casting animation
    public void OnCastAnimationEnd()
    {
        CastFireball();
        isCasting = false; // Indicate that casting is finished

        // If there is a queued spell, cast it
        if (!string.IsNullOrEmpty(queuedSpellId))
        {
            CastSpell(queuedSpellId);
            queuedSpellId = null;
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
            // Place it at the current player position when activated and set it to despawn after 5 seconds
            if (!barrierInstance.activeSelf)
            {
                barrierInstance.transform.position = playerTransform.position; // Set the position only when activating
                barrierInstance.SetActive(true);
                Invoke("DeactivateBarrier", 6.0f); // Schedule deactivation in 6 seconds
            }
            else
            {
                barrierInstance.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Barrier instance not found.");
        }
    }

    // Method to deactivate the barrier
    void DeactivateBarrier()
    {
        if (barrierInstance != null)
        {
            barrierInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Press B to cast barrier
        {
            CastSpell("uruz");
        }
        if (Input.GetKeyDown(KeyCode.T)) // Press T to cast fireball
        {
            CastSpell("kenaz");
        }
    }
}
