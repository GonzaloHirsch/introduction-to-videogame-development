using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public Constants.AUDIO_TYPE type;

    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch = 1f; // default

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
