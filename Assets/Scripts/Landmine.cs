using UnityEngine;

public class Landmine : ExplosiveBase
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy detected");
            TriggerExplosion();
        }
    }
}
