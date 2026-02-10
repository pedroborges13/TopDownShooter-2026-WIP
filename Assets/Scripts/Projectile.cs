using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;
    private Rigidbody rb;

    //Internal variables to store data from WeaponData
    private float damage;
    private float knockback;
    private int currentPierce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(WeaponData data)
    {
        damage = data.Damage;
        knockback = data.KnockbackForce;  
        currentPierce = data.PierceCount;

        rb.linearVelocity = transform.forward * data.ProjectileSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(damage, transform.forward, knockback);

            if(currentPierce <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                currentPierce--; //Loses 1 "pierce" when passing through an enemy
            }
        }
        
        if (other.CompareTag("ExplosiveBarrel"))
        {
            other.GetComponent<ExplosiveBarrel>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
