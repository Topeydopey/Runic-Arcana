using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Animator chestAnimator;
    public string devourTriggerName = "DevourTrigger";
    public float interactionRange = 2f;
    public Transform chestTransform;
    public GameObject player; // Reference to the player GameObject
    public string nextSceneName; // Name of the next scene to load

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceToChest = Vector3.Distance(player.transform.position, chestTransform.position);
            if (distanceToChest <= interactionRange)
            {
                // Trigger the devour animation
                chestAnimator.SetTrigger(devourTriggerName);

                // Start the coroutine to disable the player and transition to the next level
                StartCoroutine(DevourPlayerAndTransition());
            }
        }
    }

    private IEnumerator DevourPlayerAndTransition()
    {
        // Disable the player GameObject
        player.SetActive(false);

        // Wait for 5 seconds
        yield return new WaitForSeconds(15f);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
