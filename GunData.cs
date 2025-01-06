using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;
    public int damage;   
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public GameObject bulletImpactEffect;   

    [Header("Recoil Settings")]
    public float recoilX; // vertical
    public float recoilY; // horizontal
    public float recoilZ; // backward
    public float recoilReturnSpeed = 2f;

    [Header("Ammo Settings")]
    public int reserveAmmo;
}
