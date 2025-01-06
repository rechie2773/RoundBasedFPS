using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MasterAudioSource : MonoBehaviour
{
    public Slider slider;
    private List<AudioSource> audioSources;
    void Awake()
    {
        audioSources = new List<AudioSource>();
        // Find all AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            audioSources.Add(source);
        }
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            Debug.Log("Slider assigned and listener added.");
        }
        else
        {
            Debug.LogError("Slider is not assigned in inspector");
        }
    }
    void Start()
    {
        if (slider != null)
        {
            slider.value = 0.25f;
            // Apply the initial slider value to all audio sources
            OnValueChanged(slider.value);
            Debug.Log("Slider initialized to max value.");
        }
    }
    public void OnValueChanged(float value)
    {
        Debug.Log("Slider value changed: " + value);
        foreach (AudioSource source in audioSources)
        {
            source.volume = value;
            Debug.Log("AudioSource volume set to: " + source.volume);
        }
    }
    public void RegisterAudioSource(AudioSource newSource)
    {
        if (!audioSources.Contains(newSource))
        {
            audioSources.Add(newSource);
            newSource.volume = slider.value; // Set the volume based on the current slider value
        }
    }
    public void UpdateAudioSources()
    {
        audioSources.Clear(); // Clear the old list
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>(); // Get all active AudioSources
        foreach (AudioSource source in allAudioSources)
        {
            audioSources.Add(source);
        }

        // Apply current slider value to all sources
        float currentVolume = slider.value;
        foreach (AudioSource source in audioSources)
        {
            source.volume = currentVolume;
        }
    }
}
