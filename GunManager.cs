using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
public class GunManager : MonoBehaviour
{
    [Header("Weapon Models")]
    public List<GameObject> weaponModels; // weapon models

    [Header("Gun Settings")]
    public List<GunData> guns; // guns list
    public int currentGunIndex = 0; // guns using

    [Header("Gun Stats")]
    [HideInInspector]
    public GunData currentGun; // current gun data
    private int bulletsLeft;
    private int bulletsShot;

    [Header("References")]
    public ADS adsScript;
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask whatIsEnemy;

    [Header("UI")]
    public TextMeshProUGUI ammoText; // ammo display
    public TextMeshProUGUI gunNameText; // gun display

    [Header("Graphics")]
    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;

    private GunAudioManager gunAudioManager; //audio manager

    private bool readyToShoot = true;
    private bool reloading = false;
    private bool shooting;

    private Vector3 currentRecoil; // current recoil
    private Vector3 targetRecoil; // target recoil
    private PlayerCam playerCam; // camera shake

    private int reserveAmmo;
    public GunAnims gunAnims;
    private bool instaKillActive = false;
    private float currentSpread;
    [Header("Runtime Stats")]
    private Dictionary<GunData, int> runtimeDamage = new Dictionary<GunData, int>();
    [Header("Insta Kill UI")]
    public TextMeshProUGUI instaKillTimerText;
    private MasterAudioSource masterAudioSource;

    private void Start()
    {
        if (guns.Count > 0) // check if gun list is emptyy
        {
            currentGunIndex = 0; // ensure that the first weapon is m1911
            EquipGun(currentGunIndex);
        }
        else
        {
            Debug.LogError("No guns assigned in the GunManager!");
        }
        foreach (GunData gun in guns)
        {
            runtimeDamage[gun] = gun.damage; // Store the original damage in runtime dictionary
        }
        playerCam = FindObjectOfType<PlayerCam>();
        masterAudioSource = FindObjectOfType<MasterAudioSource>();
        if (masterAudioSource != null && gunAudioManager != null)
        {
            if (gunAudioManager.fireAudioSource != null)
            {
                masterAudioSource.RegisterAudioSource(gunAudioManager.fireAudioSource);
            }
            if (gunAudioManager.reloadAudioSource != null)
            {
                masterAudioSource.RegisterAudioSource(gunAudioManager.reloadAudioSource);
            }
        }
    }
    private void Update()
    {
        if (PauseMenu.paused) return;
        HandleInput();
        UpdateUI();

        // Apply recoil for camera
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * currentGun.recoilReturnSpeed);
        fpsCam.transform.localEulerAngles -= currentRecoil * Time.deltaTime * 50f;
    }

    public void EquipGun(int index)
    {
        if (index < 0 || index >= guns.Count) return;

        // Hide all guns
        foreach (var model in weaponModels)
        {
            if (model != null)
                model.SetActive(false);
        }

        // Show selected gun
        GameObject currentGunModel = weaponModels[index];
        if (currentGunModel != null)
        {
            currentGunModel.SetActive(true);
        }

        currentGun = guns[index];
        currentGunIndex = index;
        bulletsLeft = currentGun.magazineSize;
        readyToShoot = true;
        reserveAmmo = currentGun.reserveAmmo; // Set reserve ammo
        // Update gun UI
        if (gunNameText != null)
            gunNameText.text = currentGun.gunName;

        if (currentGunModel != null)
        {
            currentGunModel.SetActive(true);
         
            Animator gunAnimator = currentGunModel.GetComponent<Animator>();
            if (gunAnimator != null)
            {
                gunAnims.SetAnimator(gunAnimator);
            }
            gunAudioManager = currentGunModel.GetComponent<GunAudioManager>();
            if (gunAudioManager == null)
            {
                Debug.LogWarning($"GunAudioManager is missing on {currentGunModel.name}");
            }

        }
        adsScript = FindObjectOfType<ADS>();
        MasterAudioSource masterAudioSource = FindObjectOfType<MasterAudioSource>();
        if (masterAudioSource != null)
        {
            masterAudioSource.UpdateAudioSources(); // Update audio sources for the new gun
        }
    }

    private void HandleInput()
    {
        if (PauseMenu.paused) return;


        // Fire check
        if (currentGun.allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0); // Fire endlessly
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0); // Fire once
        }

        if (Input.GetButton("Fire1") && readyToShoot && !reloading && bulletsLeft > 0)
        {
            shooting = true;
            Shoot();
        }
        else if (Input.GetButtonUp("Fire1")) 
        {
            shooting = false;
            gunAnims?.StopShootAnimation(); 
        }
         

        // Reload check
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < currentGun.magazineSize && reserveAmmo > 0 && !reloading)
        {
            Reload();
        }

        // Shoot if ready
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = currentGun.bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        muzzleFlash.Play();

        gunAudioManager?.PlayFireSound();
        gunAnims?.PlayShootAnimation();

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            Invoke(nameof(DisableMuzzleFlashLight), currentGun.timeBetweenShooting);
        }
        if (adsScript != null && adsScript.IsAiming)
        {
            currentSpread = currentGun.spread * 0.05f; 
        }
        else
        {
            currentSpread = currentGun.spread;
        }

        for (int i = 0; i < currentGun.bulletsPerTap; i++)
        {
            float x = Random.Range(-currentSpread, currentSpread);
            float y = Random.Range(-currentSpread, currentSpread);
            Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

            if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit hit, currentGun.range, whatIsEnemy))
            {
                // Check if the hit object has a DestructibleObject
                DestructibleObject destructible = hit.collider.GetComponent<DestructibleObject>();
                if (destructible != null)
                {
                    destructible.TakeDamage(currentGun.damage);
                }

                // Handle enemy hit logic
                EnemyAI enemyAI = hit.collider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(currentGun.damage);
                }

                if (currentGun.bulletImpactEffect != null)
                {
                    GameObject impact = Instantiate(currentGun.bulletImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 0.2f); // Destroy the impact effect after 0.2 seconds
                }
            }
        }

        bulletsLeft--;
        bulletsShot--;

        // Camera shake effect
        if (playerCam != null)
        {
            StartCoroutine(playerCam.Shake(0.1f, 0.05f));
        }

        // Apply recoil
        targetRecoil += new Vector3(currentGun.recoilX, Random.Range(-currentGun.recoilY, currentGun.recoilY), currentGun.recoilZ);

        Invoke(nameof(ResetShot), currentGun.timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), currentGun.timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (reserveAmmo <= 0 || bulletsLeft == currentGun.magazineSize)
        {
            Debug.Log("No ammo to reload!");
            return;
        }

        reloading = true;

        gunAnims?.PlayReloadAnimation();

        gunAudioManager?.PlayReloadSound();

        Invoke(nameof(FinishReload), currentGun.reloadTime);
    }

    private void FinishReload()
    {
        int bulletsNeeded = currentGun.magazineSize - bulletsLeft;
        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);

        bulletsLeft += bulletsToReload;
        reserveAmmo -= bulletsToReload;

        reloading = false; 
        readyToShoot = true;
    }

    private void DisableMuzzleFlashLight()
    {
        if (muzzleFlashLight != null) muzzleFlashLight.enabled = false;
    }
    private void UpdateUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{bulletsLeft}  /  {reserveAmmo}";
        }
    }
    public void RefillAmmo()
    {
        if (reserveAmmo < currentGun.reserveAmmo)
        {
            reserveAmmo = currentGun.reserveAmmo; 
        }
        bulletsLeft = currentGun.magazineSize;  // Ensure the magazine is also refilled
        UpdateUI(); // Update the UI to reflect changes
    }
    private IEnumerator HandleInstaKill(float duration)
    {
        float timer = duration;

        // Show the timer UI
        if (instaKillTimerText != null)
        {
            instaKillTimerText.gameObject.SetActive(true);
        }

        while (timer > 0)
        {
            // Update the timer text
            if (instaKillTimerText != null)
            {
                instaKillTimerText.text = $"Insta Kill: {timer:F1}s";
            }

            yield return new WaitForSeconds(0.1f); // Update every 0.1 second
            timer -= 0.1f;
        }

        // Hide the timer UI
        if (instaKillTimerText != null)
        {
            instaKillTimerText.gameObject.SetActive(false);
        }

        // Reset the damage
        foreach (GunData gun in guns)
        {
            if (runtimeDamage.ContainsKey(gun))
            {
                gun.damage = runtimeDamage[gun]; // Restore original damage
            }
        }

        instaKillActive = false;
        Debug.Log("InstaKill Ended!");
    }
    public void ActivateInstaKill(float duration)
    {
        if (instaKillActive) return; // Prevent duplicate activation
        instaKillActive = true;

        foreach (GunData gun in guns)
        {
            runtimeDamage[gun] = gun.damage; // Save current damage
            gun.damage = 9999; // Set insta-kill damage
        }

        Debug.Log("InstaKill Activated!");
        StartCoroutine(HandleInstaKill(duration));
    }
}
