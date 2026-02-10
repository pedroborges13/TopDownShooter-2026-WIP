using UnityEngine;
using UnityEngine.Rendering;

public class ExplosiveBarrel : ExplosiveBase
{
    [Header("Stats")]
    [SerializeField] private float maxHp;

    private float currentHp;

    public float MaxHp => maxHp;

    void Start()
    {
        currentHp = MaxHp;
    }
    public void TakeDamage(float damage)
    {
        if (hasExploded) return;

        currentHp -= damage;

        if(currentHp <= 0) TriggerExplosion();
    }
}
