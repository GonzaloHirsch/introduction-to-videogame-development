using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public ParticleSystem explosionSystem;
    public Constants.AUDIO_TYPE audioType;

    void Start()
    {
        // Play the explosion sound
        AudioManager.Instance.Play(audioType);
        // Destroy
        float totalDuration = this.explosionSystem.main.duration + this.explosionSystem.main.startLifetime.constant;
        Destroy(this.gameObject, totalDuration);
    }
}
