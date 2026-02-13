using System.Collections.Generic;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private LayerMask targetLayer;
    private float radius;
    private float maxDamage;
    private float knockbackForce;

    [Header("VFX")]
    [SerializeField] private float vfxDuration;

    //List to avoid multiple damage in object with multiple colliders
    private List<GameObject> objectsDamaged = new List<GameObject>();

    public void Setup(float newRadius, float newDamage, float newKnockback)
    {
        radius = newRadius;
        maxDamage = newDamage;  
        knockbackForce = newKnockback;

        Explode();
        Destroy(gameObject, vfxDuration);
    }

    void Explode()
    {
        //OverlapSphere(Vector3 position, float radius, int layerMask)
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayer);

        float innerRadius = radius * 0.3f; //Damage fallof starts here (30% of radius). Full damage zone: 0% to 30% of explosion radius

        foreach (Collider hit in hits)
        {
            if (objectsDamaged.Contains(hit.gameObject) || (hit.transform.parent != null && objectsDamaged.Contains(hit.transform.parent.gameObject))) continue;
            objectsDamaged.Add(hit.gameObject);

            //Distance between the explosion and target
            Vector3 closestPoint = hit.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);

            //InverseLerp(A, B, value) maps distance to a 0-1 range between innerRadius and radius
            //If distance is less than innerRadius, result is 0
            //If distance is equals radius, result is 1
            float falloff = Mathf.InverseLerp(innerRadius, radius, distance);

            //Converts 0 -> 100% damage and 1 -> 0% damage
            float damagePercent = 1 - falloff;

            float finalDamage = maxDamage * damagePercent;

            if (finalDamage < 0.5f) continue; //If damage is low, ignore it

            //Knockback direction (from explosion center to enemy)
            Vector3 knockbackDir = (hit.transform.position - transform.position).normalized;
            knockbackDir.y = 0; //Stays at ground

            if (hit.TryGetComponent<EntityStats>(out EntityStats stats))
            {
                stats.TakeDamage(finalDamage, knockbackDir, knockbackForce * damagePercent);
                Debug.Log($"<color=orange>EXPLOSION:</color> {hit.name} takes {finalDamage.ToString("F2")} of damage. CurrentHp: {stats.CurrentHp}. Distance from explosion point {distance:F2}m ");
            }

            if (hit.TryGetComponent<BarrierHealth>(out BarrierHealth barrier))
            {
                barrier.TakeDamage(finalDamage);
            }
            if (hit.TryGetComponent<ExplosiveBarrel>(out ExplosiveBarrel otherBarrel))
            {
                otherBarrel.TriggerExplosion();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f); 
        Gizmos.DrawSphere(transform.position, radius);  
    }
}
