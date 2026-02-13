using UnityEngine;

public class BarrierHealth : MonoBehaviour
{
    [SerializeField] private float maxHp;

    public float MaxHp => maxHp;
    public float CurrentHp {  get; private set; }   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHp = MaxHp;
    }

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;

        if(CurrentHp <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
