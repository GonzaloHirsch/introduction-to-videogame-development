using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public Constants.AUDIO_TYPE[] bgAudioTypes = 
        new Constants.AUDIO_TYPE[]{
            Constants.AUDIO_TYPE.BG_FIRST_BEAT, 
            Constants.AUDIO_TYPE.BG_SECOND_BEAT
        };
    public float bgSoundDeltaMaxLimit = 1f;
    public float bgSoundDeltaMinLimit = 0.2f;
    public bool isBgReproducing = true;
    [Range(0.0f, 1.0f)]

    private int bgTypesIndex = 0;
    private float bgSoundTime = 0f;
    
    [SerializeField]
    private float currentBgSoundDelta;

    void Awake()
    {
        // Set bg sound delta
        this.currentBgSoundDelta = this.bgSoundDeltaMaxLimit;
    }

    void Update()
    {
        // Update BG Sound time
        this.bgSoundTime += Time.deltaTime;
        // Check if play BG sound
        if (this.isBgReproducing){
            this.PlayBgSound();
        }
    }

    public void UpdateBgSoundSpeed(float progress) {
        this.currentBgSoundDelta = this.bgSoundDeltaMaxLimit - ((this.bgSoundDeltaMaxLimit - this.bgSoundDeltaMinLimit) * progress);
    }

    // Play the background music
    void PlayBgSound() {
        if (this.bgSoundTime >= this.currentBgSoundDelta) {
            AudioManager.Instance.Play(this.bgAudioTypes[this.bgTypesIndex]);
            this.bgTypesIndex = (this.bgTypesIndex + 1) % this.bgAudioTypes.Length;
            this.bgSoundTime = 0;
        }
    }
}
