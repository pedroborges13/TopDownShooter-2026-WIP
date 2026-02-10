using UnityEngine;

public abstract class ExplosiveBase : MonoBehaviour
{
    [Header("Base Explosion Settings")]
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected float radius;
    [SerializeField] protected float damage;
    [SerializeField] protected float knockback;

    protected bool hasExploded = false;

    public virtual void TriggerExplosion()
    {
        if (hasExploded) return;
        hasExploded = true;

        GameObject vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        vfx.GetComponent<Explosion>().Setup(radius, damage, knockback);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
