using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject hitmarker; 

    [Header("Hitmarker Settings")]
    public float displayDuration = 0.3f; 

    private void Start()
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(false); 
        }
    }

    public void ShowHitmarker()
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(true); 
            CancelInvoke(nameof(HideHitmarker)); 
            Invoke(nameof(HideHitmarker), displayDuration);
        }
    }

    private void HideHitmarker()
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(false); 
        }
    }

}
