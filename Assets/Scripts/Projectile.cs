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

        rb.linearVelocity = transform.forward * data.ProjectileSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(damage, transform.forward, knockback);

            Destroy(gameObject);
        }
    }
}
