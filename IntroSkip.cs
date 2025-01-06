using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroSkip : MonoBehaviour
{
    [Header("Skip Settings")]
    public float holdTimeToSkip = 2f; 
    private float holdTime = 0f; 

    [Header("UI Elements")]
    public GameObject skipUI; 
    public Slider progressSlider; 
    public TextMeshProUGUI skipText; 

    [Header("Next Scene")]
    public string nextSceneName = "FPS_RoundBased"; 

    private void Start()
    {
        if (skipUI != null)
        {
            skipUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            holdTime += Time.deltaTime;

            if (skipUI != null)
            {
                skipUI.SetActive(true);
                progressSlider.value = holdTime / holdTimeToSkip; 
                skipText.text = $"Hold T to Skip ({Mathf.Ceil(holdTimeToSkip - holdTime)}s)";
            }

            if (holdTime >= holdTimeToSkip)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            holdTime = 0f;
            if (skipUI != null)
            {
                skipUI.SetActive(false);
            }
        }
    }
}
