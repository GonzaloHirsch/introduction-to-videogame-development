using UnityEngine.Audio;
using UnityEngine;
using System;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance = null;

    void Awake()
    {
        ManageSingletonInstance();

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }
    // Creating our own singleton since MonoBehaviorSingleton
    // does not work with DontDestroyOnLoad
    private void ManageSingletonInstance() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    private Sound GetSoundByType(Constants.AUDIO_TYPE audioType)
    {
        return Array.Find(sounds, sound => sound.type == audioType);
    }

    public void Play(Constants.AUDIO_TYPE audioType, bool noOverlap = false) 
    {
        Sound s = GetSoundByType(audioType);
        if (s == null) {
            Debug.LogWarning("Sound of type " + audioType + " not found!"); 
            return;
        }
        // If I want no overlap of sounds, do not want to play 
        // the audio if it is already playing
        if (noOverlap && s.source.isPlaying) {
            return;
        }
        s.source.Play();
    }

    public void Stop(Constants.AUDIO_TYPE audioType) 
    {
        Sound s = GetSoundByType(audioType);
        if (s == null) {
            Debug.LogWarning("Sound of type " + audioType + " not found!"); 
            return;
        }

        if (s.source.isPlaying) {
            s.source.Stop();
        }
    }

    // Stops all sounds, we just need to stop the looping ones, the others stop after finishing
    public void StopAll() {
        foreach(Sound sound in sounds){
            if (sound.source.isPlaying && sound.source.loop) {
                sound.source.Stop();
            }
        }
    }
}
