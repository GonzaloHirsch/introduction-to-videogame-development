using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip[] bgSounds;
    public float bgSoundDeltaMaxLimit = 1f;
    public float bgSoundDeltaMinLimit = 0.2f;
    public bool isBgReproducing = true;
    [Range(0.0f, 1.0f)]
    public float bgSoundVolume = 0.5f;

    private int bgSoundIndex = 0;
    private float bgSoundTime = 0f;
    
    [SerializeField]
    private float currentBgSoundDelta;
    private AudioSource audioSource;

    void Awake()
    {
        // Set bg sound delta
        this.currentBgSoundDelta = this.bgSoundDeltaMaxLimit;
        // Recover the audio component
        this.audioSource = GetComponent<AudioSource>();
        // Set the volume
        this.audioSource.volume = this.bgSoundVolume;
    }

    void Update()
    {
        // Update BG Sound time
        this.bgSoundTime += Time.deltaTime;
        // Check if play BG sound
        if (this.isBgReproducing){
            this.playBgSound();
        }
    }

    public void updateBgSoundSpeed(float progress) {
        this.currentBgSoundDelta = this.bgSoundDeltaMaxLimit - ((this.bgSoundDeltaMaxLimit - this.bgSoundDeltaMinLimit) * progress);
    }

    // Play the background music
    void playBgSound() {
        if (this.bgSoundTime >= this.currentBgSoundDelta) {
            this.audioSource.PlayOneShot(this.bgSounds[this.bgSoundIndex]);
            this.bgSoundIndex = (this.bgSoundIndex + 1) % this.bgSounds.Length;
            this.bgSoundTime = 0;
        }
    }
}
