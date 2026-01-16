using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;
    private float projDamage;

    public void SetDamage(float damage)
    {
        projDamage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EntityStats>().TakeDamage(projDamage);
            Destroy(gameObject);
        }
    }
}
