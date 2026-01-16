using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject projPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float bulletSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        Destroy(newProj, 3);
    }

    void OnDestroy()
    {
        PlayerController.OnShootPressed -= Shoot;
    }
}
