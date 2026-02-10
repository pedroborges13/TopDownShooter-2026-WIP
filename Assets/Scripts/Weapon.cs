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
        currentAmmo = weaponData.MagazineSize;
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
        StopAllCoroutines();
        isReloading = false;
    }

    public void TryShoot()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(ReloadRoutine());
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

        for (int i = 0; i < weaponData.ProjPerShot; i++)
        {
            float horizontalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            float verticalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(horizontalSpread, verticalSpread, 0);

            GameObject newProj = Instantiate(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation * spreadRotation);

            newProj.GetComponent<Projectile>().Setup(weaponData);
        }
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
}
