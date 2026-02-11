using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform muzzlePoint;

    [Header("VFX")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float muzzleFlashDuration;

    [Header("Aim Layer")]
    [SerializeField] private LayerMask aimLayerMask;

    private float fireTime;
    private int currentAmmo;
    private bool isReloading;

    public bool IsAutomatic => weaponData.IsAutomatic;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponData.MagazineSize;
    public bool IsReloading => isReloading;

    public static event Action<string> OnWeaponEquipped; //name
    public static event Action<int, int> OnAmmoChanged; //current, max
    public static event Action<float> OnReloadStart; //reload time


    void Start()
    {
        PlayerController.OnWeaponReloaded += Reload;

        currentAmmo = weaponData.MagazineSize;

        //Pistol
        OnWeaponEquipped?.Invoke(weaponData.WeaponName);
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
    }

    public int GetPrice()
    {
        return weaponData.Price;
    }
    public void OnEquip()
    {
        gameObject.SetActive(true);
        isReloading = false;

        OnWeaponEquipped?.Invoke(weaponData.WeaponName);
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
    }

    public void OnUnequip()
    {
        gameObject.SetActive(false);
        StopAllCoroutines(); //Stops reload if player switches weapons
        isReloading = false;
    }

    public void TryShoot()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        if (Time.time >= fireTime)
        {
            Shoot();
            fireTime = Time.time + weaponData.FireInterval;
        }
    }

    void Shoot()
    {
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
        SpawnMuzzleFlash();

        //The animations were shaking the rotation too much, so it was necessary to base it on the player's rotation
        Quaternion playerRotation = transform.root.rotation;

        for (int i = 0; i < weaponData.ProjPerShot; i++)
        {
            //Calculate random spread based on weapon data
            float horizontalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            float verticalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(horizontalSpread, verticalSpread, 0);

            //Combine the stable player rotation with the randomized spred
            Quaternion finalRotarion = playerRotation * spreadRotation;

            //Instantiate the projectile at the muzzle position, but using the stabilized rotation
            GameObject newProj = Instantiate(weaponData.ProjectilePrefab, muzzlePoint.position, finalRotarion);

            newProj.GetComponent<Projectile>().Setup(weaponData);
        }
    }

    void Reload()
    {
        if (currentAmmo >= weaponData.MagazineSize) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        OnReloadStart?.Invoke(weaponData.ReloadTime);

        yield return new WaitForSeconds(weaponData.ReloadTime);

        currentAmmo = weaponData.MagazineSize;
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
        isReloading = false;
    }

    void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab  != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);

            Destroy(flash, muzzleFlashDuration);
        }
    }

    void OnDestroy()
    {
        PlayerController.OnWeaponReloaded -= Reload;
    }
}
