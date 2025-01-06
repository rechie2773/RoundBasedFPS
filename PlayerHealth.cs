using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public float maxHealth = 200f;
    public float health;

    private float regenRate = 50f;
    private float regenDelay = 8f;
    private float regenTimer;
    private bool isRegenerating = false;

    [Header("Damage Feedback")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip lowHealthWarningSound;
    public Image damageScreen;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.3f);

    private bool isTakingDamage = false;
    private bool isLowHealthWarningPlaying = false;

    [Header("Game Over Settings")]
    public float gameOverDelay = 25f; // Delay before switching scene
    public Camera playerCamera; // Player's camera
    public Camera gameOverCamera1; // First Game Over camera
    public Camera gameOverCamera2; // Second Game Over camera
    public GameObject gameplayUI; // Gameplay UI
    public GameObject gameOverUI; // Game Over UI
    public bool isDead = false;

    private void Start()
    {
        health = maxHealth;
        if (damageScreen != null)
        {
            damageScreen.color = new Color(0, 0, 0, 0); // Clear initial screen flash color
        }       
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        regenTimer = regenDelay;
        isRegenerating = false;

        if (damageScreen != null)
        {
            damageScreen.color = flashColor; // Show red flash effect
        }

        if (audioSource && damageSound)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        HandleHealthRegeneration();
        HandleDamageScreenEffect();
        HandleLowHealthWarning();
    }

    private void HandleHealthRegeneration()
    {
        if (!isRegenerating)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0)
            {
                isRegenerating = true;
            }
        }

        if (isRegenerating && health < maxHealth)
        {
            health = Mathf.Clamp(health + (regenRate * Time.deltaTime), 0, maxHealth);
        }
    }

    private void HandleDamageScreenEffect()
    {
        if (damageScreen != null)
        {
            damageScreen.color = Color.Lerp(damageScreen.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    private void HandleLowHealthWarning()
    {
        if (health <= maxHealth * 0.25f)
        {
            if (!isLowHealthWarningPlaying)
            {
                if (audioSource && lowHealthWarningSound)
                {
                    audioSource.loop = true;
                    audioSource.clip = lowHealthWarningSound;
                    audioSource.Play();
                    isLowHealthWarningPlaying = true;
                }
            }
        }
        else
        {
            if (isLowHealthWarningPlaying)
            {
                audioSource.Stop();
                isLowHealthWarningPlaying = false;
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died");

        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.HandleGameOver(); 
        }
        //disable player's camera
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        //disable players collider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
