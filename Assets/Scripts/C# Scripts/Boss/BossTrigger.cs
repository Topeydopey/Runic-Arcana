using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossEnemy boss; // Reference to the BossEnemy script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the trigger."); // Debug statement
            boss.ActivateBoss();
            //    gameObject.SetActive(false); // Disable the trigger to prevent multiple activations
        }
    }
}
