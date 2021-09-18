using UnityEngine.Audio;
using UnityEngine;
using System;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    // Update is called once per frame
    void Play(Constants.SOUND_TYPE soundType) 
    {
        Sound s = Array.Find(sounds, sound => sound.type == soundType);
        if (s == null) {
            Debug.LogWarning("Sound of type " + soundType + " not found!"); 
            return;
        }
        s.source.Play();
    }
}
