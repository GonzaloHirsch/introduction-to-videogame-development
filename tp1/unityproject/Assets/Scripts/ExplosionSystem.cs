using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public ParticleSystem explosionSystem;

    void Start()
    {
        // Destroy
        float totalDuration = this.explosionSystem.main.duration + this.explosionSystem.main.startLifetime.constant;
        Destroy(this.gameObject, totalDuration);
    }
}
