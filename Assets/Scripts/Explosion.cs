using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float radius;
    [SerializeField] private float maxDamage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private LayerMask targetLayer;

    [Header("VFX")]
    [SerializeField] private float vfxDuration;

    void Start()
    {
        Explode();

        Destroy(gameObject, vfxDuration);
    }

    public void Setup(float newRadius, float newDamage, float newKnockback)
    {
        radius = newRadius;
        maxDamage = newDamage;  
        knockbackForce = newKnockback;
    }

    void Explode()
    {
        //OverlapSphere(Vector3 position, float radius, int layerMask)
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayer);

        foreach (Collider hit in hits)
        {
            //Distance between the explosion and target
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            //Falloff Calculation.Mathf.Clamp01 ensures the value stays between 0 and 1 (0% to 100%)
            //If distance is 0, percent is 1. If distance equals radius, percent is 0.
            float damagePercent = Mathf.Clamp01(1 - (distance/radius));

            float finalDamage = maxDamage * damagePercent;

            if (finalDamage < 1f) continue;

            //Knockback direction (from explosion center to enemy)
            Vector3 knockbackDir = (hit.transform.position - hit.transform.position).normalized;
            knockbackDir.y = 0; //Stays at ground

            if (hit.TryGetComponent<EntityStats>(out EntityStats stats))
            {
                stats.TakeDamage(finalDamage, knockbackDir, knockbackForce * damagePercent);    
            }

            if (hit.TryGetComponent<BarrierHealth>(out BarrierHealth barrier))
            {
                barrier.TakeDamage(finalDamage);
            }
            /* if (hit.TryGetComponent<ExplosiveBarrel>(out ExplosiveBarrel otherBarrel))
            {
                otherBarrel.TriggerExplosion();
            }*/
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f); 
        Gizmos.DrawSphere(transform.position, radius);  
    }
}
