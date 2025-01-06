using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Import SceneManager for scene loading

public class TriggerSceneTransition : MonoBehaviour
{
    public TextMeshProUGUI TriggerText;
    // Scene name to load after the delay
    public string sceneToLoad;
    // Fade-in and fade-out durations
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 3f;

    // Scene transition delay after text fade-out
    public float sceneTransitionDelay = 5f;

    void Start()
    {
        // Check if text object is assigned
        if (TriggerText == null)
        {
            Debug.LogError("TextMeshPro object not assigned in inspector!");
        }

        // Initialize text alpha to 0 (hidden)
        TriggerText.alpha = 0f;
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check if colliding object is the player
        if (collider.gameObject.tag == "Player")
        {
            // Start fade-in text coroutine
            StartCoroutine(FadeInText());

            // Start scene transition timer coroutine
            StartCoroutine(SceneTransitionTimer());
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

        // Destroy game object after fade out
        Destroy(gameObject);
    }

    IEnumerator SceneTransitionTimer()
    {
        yield return new WaitForSeconds(sceneTransitionDelay); // Wait for delay

        // Load the specified scene
        SceneManager.LoadScene("MissionSuccess");
    }
}
