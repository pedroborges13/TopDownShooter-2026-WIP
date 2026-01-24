using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Visuals")]
    [SerializeField] private string weaponName;
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRateRPM;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private bool isAutomatic;

    //Visuals
    public string WeaponName => weaponName;
    public GameObject ModelPrefab => modelPrefab;
    public GameObject ProjectilePrefab => projectilePrefab;

    //Stats
    public float Damage => damage;
    public float FireInterval => 60f / fireRateRPM; //Já entrega o cálculo pronto para o script Weapon
    public float KnockbackForce => knockbackForce;
    public float ProjectileSpeed => projectileSpeed;
    public bool IsAutomatic => isAutomatic;
}
