using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform muzzlePoint;

    private float fireTime;

    public bool IsAutomatic => weaponData.IsAutomatic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

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
        GameObject newProj = Instantiate(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);

        newProj.GetComponent<Projectile>().Setup(weaponData);
    }

    void OnDestroy()
    {

    }
}
