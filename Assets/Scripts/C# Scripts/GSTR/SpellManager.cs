using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class SpellManager : MonoBehaviour
{
    public CursorRotationAim cursorRotationAim;
    public GameObject barrierPrefab;
    public GameObject lightningBoltPrefab;
    public GameObject waterWavePrefab;
    public Transform playerTransform;
    public Light2D manaLight;
    public float maxMana = 100f;
    public float fireballManaCost = 20f;
    public float lightningManaCost = 30f;
    public float waterWaveManaCost = 15f;
    public float manaRegenRate = 10f;

    public AudioClip spellSelectSound;
    public AudioClip fireballCastSound;
    public AudioClip lightningCastSound;
    public AudioClip waterWaveCastSound;
    public AudioClip barrierActivateSound;
    public AudioClip barrierDeactivateSound;
    public AudioClip lowManaSound;
    private AudioSource audioSource;

    private GameObject barrierInstance;
    private Animator playerAnimator;
    private bool isCasting = false;
    private string queuedSpellId;
    private Animator barrierAnimator;
    private Coroutine castingCoroutine;
    private float currentMana;
    private bool fireballSelected = false;
    private bool lightningSelected = false;
    private bool waterWaveSelected = false;

    private Color lowManaColor = Color.red;
    private Color highManaColor = Color.blue;

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

        if (waterWavePrefab == null)
        {
            Debug.LogError("Water wave prefab not assigned.");
        }
        else
        {
            Debug.Log("Water wave prefab assigned successfully.");
        }

        if (manaLight == null)
        {
            Debug.LogError("Mana Light is not assigned.");
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        currentMana = maxMana;
        UpdateManaIndicator();
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
            if (currentMana >= fireballManaCost)
            {
                StartCoroutine(CastFireballRoutine());
            }
            else
            {
                PlaySound(lowManaSound);
            }
        }

        if (lightningSelected && Input.GetMouseButtonDown(0))
        {
            if (currentMana >= lightningManaCost)
            {
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SpawnLightningBolt(worldPosition);
            }
            else
            {
                PlaySound(lowManaSound);
            }
        }

        if (waterWaveSelected && Input.GetMouseButtonDown(0))
        {
            if (currentMana >= waterWaveManaCost)
            {
                StartCoroutine(CastWaterWaveRoutine());
            }
            else
            {
                PlaySound(lowManaSound);
            }
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
        if (Input.GetKeyDown(KeyCode.G)) // Changed from W to G for laguz
        {
            CastSpell("laguz");
        }
    }

    public void CastSpell(string spellId)
    {
        if (isCasting)
        {
            queuedSpellId = spellId;
            return;
        }

        // Play spell selection sound
        PlaySound(spellSelectSound);

        // Reset spell selections
        fireballSelected = false;
        lightningSelected = false;
        waterWaveSelected = false;

        switch (spellId)
        {
            case "kenaz":
                fireballSelected = true;
                break;
            case "uruz":
                ToggleBarrier();
                break;
            case "restart":
                RestartLevel();
                break;
            case "thurisaz":
                ActivateLightningPower();
                break;
            case "laguz":
                waterWaveSelected = true;
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

        // Play fireball cast sound
        PlaySound(fireballCastSound);

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

    private IEnumerator CastWaterWaveRoutine()
    {
        isCasting = true;
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Cast");
            yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
        CastWaterWave();
        isCasting = false;

        if (!string.IsNullOrEmpty(queuedSpellId))
        {
            string spellToCast = queuedSpellId;
            queuedSpellId = null;
            CastSpell(spellToCast);
        }
    }

    private void CastWaterWave()
    {
        if (currentMana < waterWaveManaCost)
        {
            Debug.Log("Not enough mana to cast water wave.");
            return;
        }

        currentMana -= waterWaveManaCost;
        UpdateManaIndicator();

        // Play water wave cast sound
        PlaySound(waterWaveCastSound);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition.x < playerTransform.position.x) ? Vector2.left : Vector2.right;

        GameObject waterWave = Instantiate(waterWavePrefab, cursorRotationAim.projectileSpawnPoint.transform.position, Quaternion.identity);
        waterWave.GetComponent<WaterWave>().Initialize(direction);

        // Flip the sprite if casting to the left
        if (direction == Vector2.left)
        {
            waterWave.transform.localScale = new Vector3(-1 * Mathf.Abs(waterWave.transform.localScale.x), waterWave.transform.localScale.y, waterWave.transform.localScale.z);
        }
        else
        {
            waterWave.transform.localScale = new Vector3(Mathf.Abs(waterWave.transform.localScale.x), waterWave.transform.localScale.y, waterWave.transform.localScale.z);
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

                // Play barrier activate sound
                PlaySound(barrierActivateSound);

                Invoke("DeactivateBarrier", 5.0f);
            }
            else
            {
                barrierAnimator.SetTrigger("BarrierDown");

                // Play barrier deactivate sound
                PlaySound(barrierDeactivateSound);

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

            // Play barrier deactivate sound
            PlaySound(barrierDeactivateSound);

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

            // Play lightning cast sound
            PlaySound(lightningCastSound);

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
            manaLight.intensity = Mathf.Lerp(2, 3, currentMana / maxMana); // Set minimum intensity to 2
            manaLight.color = Color.Lerp(lowManaColor, highManaColor, currentMana / maxMana);
        }
    }

    // Function to modify mana, can be called from other scripts
    public void ModifyMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UpdateManaIndicator();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
