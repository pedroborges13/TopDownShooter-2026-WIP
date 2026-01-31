using UnityEngine;

public class BarrierHealth : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currentHp;

    public float MaxHp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        damage -= currentHp;

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
