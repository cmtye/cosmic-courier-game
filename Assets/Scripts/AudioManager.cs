using UnityEngine;
using Utility;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource _source;

    public void PlaySound(AudioClip clip, float volume)
    {
        if (volume > 1) volume = 1;
        if (volume < 0) volume = 0;
        _source.PlayOneShot(clip, volume);
    }
}
