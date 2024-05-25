using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossEnemy boss; // Reference to the BossEnemy script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //    boss.ActivateBoss();
            gameObject.SetActive(false); // Disable the trigger to prevent multiple activations
        }
    }
}
