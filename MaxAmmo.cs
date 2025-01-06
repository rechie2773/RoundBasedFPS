using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAmmo : MonoBehaviour
{
    [Header("Announcer Settings")]
    public AudioClip announcerClip;
    public float announcerVolume = 0.25f;
    public float destroyAfter = 15f;
    private MasterAudioSource masterAudioSource;
    private void Start()
    {
        masterAudioSource = FindObjectOfType<MasterAudioSource>();
        if (masterAudioSource != null && masterAudioSource.slider != null)
        {
            announcerVolume = masterAudioSource.slider.value; // Initialize with slider value
            masterAudioSource.slider.onValueChanged.AddListener(UpdateAnnouncerVolume);
        }
        // Schedule destruction after the specified time
        Invoke(nameof(AutoDestroy), destroyAfter);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered by: {other.gameObject.name}"); // Debug to confirm collision

        if (other.CompareTag("Player"))
        {
            GunManager gunManager = FindObjectOfType<GunManager>();
            if (gunManager != null)
            {
                gunManager.RefillAmmo();
                Debug.Log("Max Ammo Triggered!");
                PlayAnnouncerAudio();
            }
            else
            {
                Debug.LogError("GunManager not found on Player!");
            }

            Destroy(gameObject); // Destroy the Power-Up after use
        }
    }
    private void PlayAnnouncerAudio()
    {
        PowerUpAnnouncer announcer = FindObjectOfType<PowerUpAnnouncer>();
        if (announcer != null)
        {
            announcer.PlayAnnouncerClip(announcerClip, announcerVolume);
        }
        else
        {
            Debug.LogWarning("No PowerUpAnnouncer found in the scene!");
        }
    }
    private void AutoDestroy()
    {
        Debug.Log("Insta-Kill expired!");
        Destroy(gameObject); // Destroy if not triggered within the time limit
    }
    private void UpdateAnnouncerVolume(float value)
    {
        announcerVolume = value;
    }

    private void OnDestroy()
    {
        if (masterAudioSource != null && masterAudioSource.slider != null)
        {
            masterAudioSource.slider.onValueChanged.RemoveListener(UpdateAnnouncerVolume);
        }
    }
}
