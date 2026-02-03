using UnityEngine;

public class BarrierHealth : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currentHp;

    public float MaxHp => maxHp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = MaxHp;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if(currentHp <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
