using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;
    private Rigidbody rb;
    private TrailRenderer trail;

    //Internal variables to store data from WeaponData
    private float damage;
    private float knockback;
    private float speed;
    private int currentPierce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
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
        speed = data.ProjectileSpeed;

        rb.linearVelocity = transform.forward * data.ProjectileSpeed;
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.1f) return;

        float travelDistance = speed * Time.fixedDeltaTime;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, travelDistance))
        {
            transform.position = hit.point;
            rb.linearVelocity = Vector3.zero;
            ProcessCollision(hit.collider);
        }
    }

    void ProcessCollision(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(damage, transform.forward, knockback);

            if (currentPierce <= 0)
            {
                StopProjectileVisually();
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

    void StopProjectileVisually()
    {
        if (trail != null) trail.emitting = false;

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        Destroy(gameObject, 0.01f);
    }

    /*void OnTriggerEnter(Collider other)
    {
        // Se bater em algo que não seja Trigger (como o chão ou paredes)
        if (!other.isTrigger && !other.CompareTag("Player"))
        {
            // Se for inimigo ou barril, o Raycast no FixedUpdate já deve ter resolvido,
            // mas paredes e chão não costumam falhar com triggers normais.
            if (!other.CompareTag("Enemy") && !other.CompareTag("ExplosiveBarrel"))
            {
                Destroy(gameObject);
            }
        }
    }*/
}
