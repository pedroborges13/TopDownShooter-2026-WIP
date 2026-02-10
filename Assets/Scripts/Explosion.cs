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

        foreach (Collider hit in hits)
        {
            if (objectsDamaged.Contains(hit.gameObject) || (hit.transform.parent != null && objectsDamaged.Contains(hit.transform.parent.gameObject))) continue;
            objectsDamaged.Add(hit.gameObject);

            //Distance between the explosion and target
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            //Falloff Calculation.Mathf.Clamp01 ensures the value stays between 0 and 1 (0% to 100%)
            //If distance is 0, percent is 1. If distance equals radius, percent is 0.
            float damagePercent = Mathf.Clamp01(1 - (distance/radius));

            float finalDamage = maxDamage * damagePercent;

            if (finalDamage < 0.5f) continue; //If damage is low, ignore it

            //Knockback direction (from explosion center to enemy)
            Vector3 knockbackDir = (hit.transform.position - transform.position).normalized;
            knockbackDir.y = 0; //Stays at ground

            if (hit.TryGetComponent<EntityStats>(out EntityStats stats))
            {
                stats.TakeDamage(finalDamage, knockbackDir, knockbackForce * damagePercent);
                Debug.Log($"<color=orange>EXPLOSÃO:</color> {hit.name} recebeu {finalDamage.ToString("F2")} de dano.");
                Debug.Log($"{hit.name} currentHp: {stats.CurrentHp}");
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
