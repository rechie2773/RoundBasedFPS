using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnnouncer : MonoBehaviour
{
    private AudioSource audioSource;
    private MasterAudioSource masterAudioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        masterAudioSource = FindObjectOfType<MasterAudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing on the PowerUpAnnouncer GameObject.");
        }

        if (masterAudioSource != null)
        {
            masterAudioSource.slider.onValueChanged.AddListener(UpdateVolume);
            UpdateVolume(masterAudioSource.slider.value); // Set initial volume
        }
        else
        {
            Debug.LogWarning("MasterAudioSource not found in the scene.");
        }
    }

    public void PlayAnnouncerClip(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Clip is missing or AudioSource is not set.");
        }
    }

    private void UpdateVolume(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value; // Update volume from slider
        }
    }

    private void OnDestroy()
    {
        if (masterAudioSource != null && masterAudioSource.slider != null)
        {
            masterAudioSource.slider.onValueChanged.RemoveListener(UpdateVolume);
        }
    }
}
