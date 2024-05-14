using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Check if the particle system has stopped playing
        if (ps && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
