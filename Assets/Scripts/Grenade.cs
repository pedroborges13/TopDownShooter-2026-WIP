using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float delay;

    [Header("Settings")]
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float force;

    void Start()
    {
        Invoke("Detonate", delay);
    }

    void Detonate()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().Setup(radius, damage, force);

        Destroy(gameObject);
    }
}
