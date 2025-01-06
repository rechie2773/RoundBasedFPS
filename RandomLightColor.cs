using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightColor : MonoBehaviour
{
    // Reference to the Pointlight component
    public Light pointLight;

    // Duration between color changes (in seconds)
    public float colorChangeDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the pointLight component is assigned
        if (pointLight == null)
        {
            Debug.LogError("PointLight component not assigned in inspector!");
            return;
        }

        // Start the coroutine to continuously change light color
        StartCoroutine(RandomColorCoroutine());
    }

    // Coroutine to continuously change light color
    IEnumerator RandomColorCoroutine()
    {
        while (true)
        {
            RandomizeLightColor();

            // Wait for the specified duration before changing color again
            yield return new WaitForSeconds(colorChangeDuration);
        }
    }

    // Randomize the color of the pointLight
    void RandomizeLightColor()
    {
        // Generate random values for red, green, and blue components
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Set the color to the pointLight
        pointLight.color = new Color(r, g, b);
    }
}

