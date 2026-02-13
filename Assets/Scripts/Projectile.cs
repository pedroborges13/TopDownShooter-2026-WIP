using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;
    private Rigidbody rb;
    private TrailRenderer trail;
    private MeshRenderer meshRenderer;

    [Header("VFX")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private float vfxDuration;

    //Internal variables to store data from WeaponData
    private float damage;
    private float knockback;
    private float speed;
    private int currentPierce;

    private IObjectPool<Projectile> _pool;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPool(IObjectPool<Projectile> pool) => _pool = pool;

    public void Setup(WeaponData data)
    {
        damage = data.Damage;
        knockback = data.KnockbackForce;  
        currentPierce = data.PierceCount;
        speed = data.ProjectileSpeed;

        rb.linearVelocity = transform.forward * data.ProjectileSpeed;

        if (meshRenderer != null) meshRenderer.enabled = true;
        if (trail != null) trail.Clear();

        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.1f) return;

        float travelDistance = speed * Time.fixedDeltaTime;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, travelDistance))
        {
            transform.position = hit.point;
            rb.linearVelocity = Vector3.zero;
            ProcessCollision(hit.collider, hit.normal);
        }
    }

    void ProcessCollision(Collider other, Vector3 hitNormal)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(damage, transform.forward, knockback);

            //VFX
            if (bloodPrefab != null)
            {
                GameObject newBlood = Instantiate(bloodPrefab, transform.position, Quaternion.LookRotation(hitNormal));
                newBlood.transform.parent = other.transform;
                Destroy(newBlood, vfxDuration);
            }

            if (currentPierce <= 0)
            {
                ReturnToPool();
                //StopProjectileVisually();
            }
            else
            {
                currentPierce--; //Loses 1 "pierce" when passing through an enemy
                rb.linearVelocity = transform.forward * speed;
                transform.position += transform.forward * 0.1f; //Prevents the projectile from getting stuck in the hit enemy
            }
        }
        else if (other.CompareTag("ExplosiveBarrel"))
        {
            other.GetComponent<ExplosiveBarrel>().TakeDamage(damage);
            ReturnToPool();
        }
        else if (other.CompareTag("Barrier"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        if (_pool != null) _pool.Release(this);
        else Destroy(gameObject);
    }

    void StopProjectileVisually()
    {
        rb.linearVelocity = Vector3.zero;
        if (trail != null) trail.emitting = false;

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        Destroy(gameObject, 0.01f);
    }
}
