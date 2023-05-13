using System;
using UnityEngine;
using UnityEngine.Audio;
using Utility;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer effectsMixer;

    public void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            musicMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        }
        else
        {
            musicMixer.SetFloat("Music", Mathf.Log10(0.5f) * 20);
        }
        
        if (PlayerPrefs.HasKey("Effects"))
        {
            effectsMixer.SetFloat("Effects", Mathf.Log10(PlayerPrefs.GetFloat("Effects")) * 20);
        }
        else
        {
            effectsMixer.SetFloat("Effects", Mathf.Log10(0.5f) * 20);
        }
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (volume > 1) volume = 1;
        if (volume < 0) volume = 0;
        source.PlayOneShot(clip, volume);
    }
}
