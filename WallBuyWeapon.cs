using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallBuyWeapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public int weaponIndex;           
    public int weaponCost = 500;      
    public string weaponName;
    public int ammoRefillCost = 250;

    private static TextMeshProUGUI sharedWeaponUI;

    private bool playerInRange = false;
    [Header("Audio")]
    public AudioSource audioSource;               
    public AudioClip enterZoneClip;               
    public AudioClip purchaseSuccessClip;         
    public AudioClip notEnoughPointsClip;


    private void Start()
    {
        // Find the shared UI text
        if (sharedWeaponUI == null)
        {
            sharedWeaponUI = GameObject.Find("WallBuyText").GetComponent<TextMeshProUGUI>();
            sharedWeaponUI.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            GunManager gunManager = FindObjectOfType<GunManager>();
            if (gunManager != null && sharedWeaponUI != null)
            {
                if (gunManager.currentGunIndex == weaponIndex)
                {
                   
                    sharedWeaponUI.text = $"Refill Ammo: {ammoRefillCost} Points - Press [F]";
                }
                else
                {
                    sharedWeaponUI.text = $"{weaponName} - Cost: {weaponCost} Points - Press [F] to Buy";
                }

                sharedWeaponUI.gameObject.SetActive(true);
            }

            PlayAudioClip(enterZoneClip);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (sharedWeaponUI != null)
            {
                sharedWeaponUI.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (GameOverManager.isGameOver) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            AttemptToBuyOrRefill();
        }
    }

    private void AttemptToBuyOrRefill()
    {
        GunManager gunManager = FindObjectOfType<GunManager>();
        if (gunManager == null) return;

        if (gunManager.currentGunIndex == weaponIndex)
        {
            // Refilling ammo
            if (PointSystem.Instance.GetPoints() >= ammoRefillCost)
            {
                PointSystem.Instance.RemovePoints(ammoRefillCost);
                gunManager.RefillAmmo();
                PlayAudioClip(purchaseSuccessClip);

                Debug.Log($"{weaponName} ammo refilled!");
                sharedWeaponUI.text = "Ammo Refilled!";
                Invoke(nameof(HideUI), 2f); // Hide UI after 2 seconds
            }
            else
            {
                Debug.Log("Not enough points to refill ammo.");
                PlayAudioClip(notEnoughPointsClip);
            }
        }
        else
        {
            // Buying the weapon
            if (PointSystem.Instance.GetPoints() >= weaponCost)
            {
                PointSystem.Instance.RemovePoints(weaponCost);
                gunManager.EquipGun(weaponIndex);
                PlayAudioClip(purchaseSuccessClip);

                Debug.Log($"{weaponName} purchased!");
                sharedWeaponUI.text = $"{weaponName} Purchased!";
                Invoke(nameof(HideUI), 2f); // Hide UI after 2 seconds
            }
            else
            {
                Debug.Log("Not enough points to buy this weapon.");
                PlayAudioClip(notEnoughPointsClip);
            }
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void HideUI()
    {
        if (sharedWeaponUI != null)
        {
            sharedWeaponUI.gameObject.SetActive(false);
        }
    }
}
