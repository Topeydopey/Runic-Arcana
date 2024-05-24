using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class SpellManager : MonoBehaviour
{
    public CursorRotationAim cursorRotationAim;
    public GameObject barrierPrefab;
    public GameObject lightningBoltPrefab; // Reference to the lightning bolt prefab
    public Transform playerTransform;
    public Light2D manaLight; // Reference to the 2D light for mana
    public float maxMana = 100f; // Maximum mana value
    public float fireballManaCost = 20f; // Mana cost for casting fireball
    public float lightningManaCost = 30f; // Mana cost for casting lightning
    public float manaRegenRate = 10f; // Mana regeneration rate per second

    private GameObject barrierInstance;
    private Animator playerAnimator;
    private bool isCasting = false;
    private string queuedSpellId;
    private Animator barrierAnimator;
    private Coroutine castingCoroutine;
    private bool isLightningPowerActive = false;
    private bool hasCastLightning = false; // Flag to check if lightning has been cast
    private float currentMana;
    private bool fireballSelected = false;
    private bool lightningSelected = false;

    // Define the colors for low and high mana
    private Color lowManaColor = Color.red;
    private Color highManaColor = Color.blue; // Full blue

    void Awake()
    {
        Debug.Log("SpellManager Awake called.");
        Debug.Log("Initial lightningBoltPrefab assignment: " + (lightningBoltPrefab != null ? lightningBoltPrefab.name : "null"));
    }

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the SpellManager.");
            return;
        }

        playerAnimator = playerTransform.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator not found on the player.");
        }

        if (barrierPrefab != null)
        {
            barrierInstance = Instantiate(barrierPrefab, playerTransform.position, Quaternion.identity);
            barrierInstance.SetActive(false);
            barrierAnimator = barrierInstance.GetComponent<Animator>();
            if (barrierAnimator == null)
            {
                Debug.LogError("Animator not found on the barrier.");
            }
        }
        else
        {
            Debug.LogError("Barrier prefab not assigned.");
        }

        if (lightningBoltPrefab == null)
        {
            Debug.LogError("Lightning bolt prefab not assigned in Start.");
        }
        else
        {
            Debug.Log("Lightning bolt prefab assigned successfully in Start.");
        }

        if (manaLight == null)
        {
            Debug.LogError("Mana Light is not assigned.");
        }

        currentMana = maxMana; // Initialize mana to max value
        UpdateManaIndicator(); // Initialize the mana indicator
    }

    void Update()
    {
        RegenerateMana();

        if (Input.GetMouseButton(1))
        {
            // Holding right click to open the UI, disable spell casting
            return;
        }

        if (fireballSelected && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CastFireballRoutine());
        }

        if (lightningSelected && Input.GetMouseButtonDown(0))
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnLightningBolt(worldPosition);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            CastSpell("uruz");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CastSpell("kenaz");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CastSpell("thurisaz");
        }
    }

    public void CastSpell(string spellId)
    {
        if (isCasting)
        {
            queuedSpellId = spellId;
            return;
        }

        switch (spellId)
        {
            case "kenaz":
                fireballSelected = true;
                lightningSelected = false;
                break;
            case "uruz":
                ToggleBarrier();
                break;
            case "restart":
                RestartLevel();
                break;
            case "thurisaz":
                ActivateLightningPower();
                fireballSelected = false;
                break;
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator CastFireballRoutine()
    {
        isCasting = true;
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Cast");
            yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
        CastFireball();
        isCasting = false;

        if (!string.IsNullOrEmpty(queuedSpellId))
        {
            string spellToCast = queuedSpellId;
            queuedSpellId = null;
            CastSpell(spellToCast);
        }
    }

    private void CastFireball()
    {
        if (currentMana < fireballManaCost)
        {
            Debug.Log("Not enough mana to cast fireball.");
            return;
        }

        currentMana -= fireballManaCost;
        UpdateManaIndicator();

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
            if (!barrierInstance.activeSelf)
            {
                barrierInstance.transform.position = playerTransform.position;
                barrierInstance.SetActive(true);
                barrierAnimator.SetTrigger("BarrierUp");
                Invoke("DeactivateBarrier", 5.0f);
            }
            else
            {
                barrierAnimator.SetTrigger("BarrierDown");
                Invoke("DisableBarrier", 1.0f);
            }
        }
        else
        {
            Debug.LogError("Barrier instance not found.");
        }
    }

    void DeactivateBarrier()
    {
        if (barrierInstance != null)
        {
            barrierAnimator.SetTrigger("BarrierDown");
            Invoke("DisableBarrier", 1.0f);
        }
    }

    void DisableBarrier()
    {
        if (barrierInstance != null)
        {
            barrierInstance.SetActive(false);
        }
    }

    private void ActivateLightningPower()
    {
        if (currentMana < lightningManaCost)
        {
            Debug.Log("Not enough mana to activate lightning power.");
            return;
        }

        lightningSelected = true;
    }

    private void SpawnLightningBolt(Vector2 position)
    {
        if (currentMana < lightningManaCost)
        {
            Debug.Log("Not enough mana to cast lightning.");
            return;
        }

        currentMana -= lightningManaCost;
        UpdateManaIndicator();

        Debug.Log("Attempting to spawn lightning bolt at position: " + position);
        if (lightningBoltPrefab != null)
        {
            Instantiate(lightningBoltPrefab, position, Quaternion.identity);
            Debug.Log("Lightning bolt instantiated at position: " + position);
        }
        else
        {
            Debug.LogError("Lightning bolt prefab not assigned in SpawnLightningBolt.");
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            UpdateManaIndicator();
        }
    }

    private void UpdateManaIndicator()
    {
        if (manaLight != null)
        {
            manaLight.intensity = Mathf.Lerp(1, 3, currentMana / maxMana); // Adjusted to minimum intensity of 1
            manaLight.color = Color.Lerp(lowManaColor, highManaColor, currentMana / maxMana);
        }
    }

    // Function to modify mana, can be called from other scripts
    public void ModifyMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UpdateManaIndicator();
    }
}
