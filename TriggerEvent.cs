using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TriggerEvent : MonoBehaviour
{
    public AudioSource audioSource; // Reference to audio source
    public AudioClip triggerSound; // Audio clip to play

    //text display
    public TextMeshProUGUI TriggerText;
    public UnityEvent onTriggerEnterEvent; // Event triggered on player enter

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 3f;
    //object trigger
    public GameObject[] objectsToDestroy;

    //trigger (once)
    private bool hasTriggered = false; // Flag to ensure the event only triggers once

    void Start()
    {
        // Check if text is assigned
        if (TriggerText == null)
        {
            Debug.LogError("TextMeshPro object not assigned in inspector!");
        }

        // Make sure text is hidden
        TriggerText.alpha = 0f; // 0 for transparency
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!hasTriggered && collider.gameObject.tag == "Player")
        {
            hasTriggered = true; // Set the flag to true to prevent further triggers

            if (audioSource && triggerSound)
            {
                audioSource.PlayOneShot(triggerSound);
            }

            // Call the onTriggerEnterEvent for additional functionality
            onTriggerEnterEvent.Invoke();

            // Start fade-in coroutine
            StartCoroutine(FadeInText());

            // Destroy objects
            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                if (objectsToDestroy[i] != null)
                {
                    Destroy(objectsToDestroy[i]);
                }
            }
        }
    }

    IEnumerator FadeInText()
    {
        float alpha = 0f;

        // Fade in text over time
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeInDuration;
            TriggerText.alpha = alpha;
            yield return null;
        }

        // Start fade-out coroutine after a delay
        yield return new WaitForSeconds(fadeOutDuration);
        StartCoroutine(FadeOutText());
    }

    IEnumerator FadeOutText()
    {
        float alpha = 1f;

        // Fade out text over time
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeOutDuration;
            TriggerText.alpha = alpha;
            yield return null;
        }

        // Destroy gameobject after fade out
        Destroy(gameObject);
    }

}
