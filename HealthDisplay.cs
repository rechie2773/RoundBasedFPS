using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class HealthDisplay : MonoBehaviour
{
    public PlayerHealth playerHealth;  
    public TextMeshProUGUI healthText; 

    void Update()
    {
        if (playerHealth != null)  // Check if reference is assigned
        {
            float healthRatio = playerHealth.health / playerHealth.maxHealth;  // Calculate health percentage
            healthText.text = "Health: " + Mathf.FloorToInt(healthRatio * 100) ;  // Update text with health %
        }
        else
        {
            Debug.LogError("PlayerHealth reference not assigned!");
        }
    }
}