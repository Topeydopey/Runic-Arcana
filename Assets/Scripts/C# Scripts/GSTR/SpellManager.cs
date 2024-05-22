using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpellManager : MonoBehaviour
{
    public CursorRotationAim cursorRotationAim;
    public GameObject barrierPrefab;
    public Transform playerTransform;
    private GameObject barrierInstance;
    private Animator playerAnimator;
    private bool isCasting = false;
    private string queuedSpellId;
    private Animator barrierAnimator;
    private Coroutine castingCoroutine;

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
                castingCoroutine = StartCoroutine(CastFireballRoutine());
                break;
            case "uruz":
                ToggleBarrier();
                break;
            case "restart":
                RestartLevel();
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CastSpell("uruz");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CastSpell("kenaz");
        }
    }
}
