using UnityEngine;

public class Grenade : ExplosiveBase
{
    [SerializeField] private float delay;

    void Start()
    {
        Invoke("TriggerExplosion", delay);
    }
}
