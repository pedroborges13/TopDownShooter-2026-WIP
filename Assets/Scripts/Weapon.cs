using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform muzzlePoint;

    private float fireTime;

    public bool IsAutomatic => weaponData.IsAutomatic;

    public int GetPrice()
    {
        return weaponData.Price;
    }
    public void OnEquip()
    {
        gameObject.SetActive(true);
    }

    public void OnUnequip()
    {
        gameObject.SetActive(false);
    }

    public void TryShoot()
    {
        if (Time.time >= fireTime)
        {
            Shoot();
            fireTime = Time.time + weaponData.FireInterval;
        }
    }

    void Shoot()
    {
        for (int i = 0; i < weaponData.ProjPerShot; i++)
        {
            float horizontalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            float verticalSpread = Random.Range(-weaponData.SpreadAngle, weaponData.SpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(horizontalSpread, verticalSpread, 0);

            GameObject newProj = Instantiate(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation * spreadRotation);

            newProj.GetComponent<Projectile>().Setup(weaponData);
        }
    }
}
