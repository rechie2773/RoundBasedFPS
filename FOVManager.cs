using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FOVManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Slider fovSlider;
    public TextMeshProUGUI fovValueText;
    public ADS adsScript; 

    private float defaultFOV = 80f;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        defaultFOV = PlayerPrefs.GetFloat("PlayerFOV", 80f);

        if (fovSlider != null)
        {
            fovSlider.minValue = 70f;
            fovSlider.maxValue = 110f;
            fovSlider.value = defaultFOV;
            fovSlider.onValueChanged.AddListener(UpdateFOV);
        }

        UpdateFOV(defaultFOV); 
    }

    private void UpdateFOV(float newFOV)
    {
        if (adsScript != null && adsScript.IsAiming)
        {
            return;
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = newFOV;
        }

        if (fovValueText != null)
        {
            fovValueText.text = $"{Mathf.RoundToInt(newFOV)}"; 
        }

        PlayerPrefs.SetFloat("PlayerFOV", newFOV);
    }

    public void ApplyFOV(float newFOV)
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = newFOV;
        }
    }
}
