using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public ParticleSystem explosionSystem;
    public AudioSource audioSource;

    void Awake() {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    void Start()
    {
        // Play the explosion sound
        this.audioSource.Play();
        // Destroy
        float totalDuration = this.explosionSystem.main.duration + this.explosionSystem.main.startLifetime.constant;
        Destroy(this.gameObject, totalDuration);
    }
}
