using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudioManager : MonoBehaviour
{
    // audio manager for gun sounds
    [Header("Audio Sources")]
    public AudioSource fireAudioSource;   
    public AudioSource reloadAudioSource; 

    //audio clips for gun sounds
    [Header("Audio Clips")]
    public AudioClip fireSound;   
    public AudioClip reloadSound; 

    // Audio settings for gun sounds
    public void PlayFireSound()
    {
        if (fireAudioSource != null && fireSound != null)
        {
            fireAudioSource.PlayOneShot(fireSound);
        }
        else
        {
            Debug.LogWarning("Fire AudioSource or Fire Sound is missing!");
        }
    }
    //play reload sound
    public void PlayReloadSound()
    {
        if (reloadAudioSource != null && reloadSound != null)
        {
            reloadAudioSource.PlayOneShot(reloadSound);
        }
        else
        {
            Debug.LogWarning("Reload AudioSource or Reload Sound is missing!");
        }
    }
    //pause audio
    public void PauseAudio()
    {
        if (fireAudioSource != null && fireAudioSource.isPlaying)
        {
            fireAudioSource.Pause();
        }
        if (reloadAudioSource != null && reloadAudioSource.isPlaying)
        {
            reloadAudioSource.Pause();
        }
    }

    // Resume all audio
    public void ResumeAudio()
    {
        if (fireAudioSource != null)
        {
            fireAudioSource.UnPause();
        }
        if (reloadAudioSource != null)
        {
            reloadAudioSource.UnPause();
        }
    }
    
}
