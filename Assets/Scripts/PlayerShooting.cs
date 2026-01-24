using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject projPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float bulletSpeed;

    //References
    EntityStats stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponent<EntityStats>();

        PlayerController.OnShootPressed += Shoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        GameObject newProj = Instantiate(projPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = newProj.GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * bulletSpeed;
        //newProj.GetComponent<Projectile>().SetDamage(stats.Damage);
    }

    void OnDestroy()
    {
        PlayerController.OnShootPressed -= Shoot;
    }
}
