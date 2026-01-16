using System;
using UnityEngine;
using UnityEngine.Rendering;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private EntityStatsData data;
    
    public float CurrentHp {  get; private set; }
    public bool IsDead { get; private set; }

    //Getters
    public float MaxHealth => data.MaxHp;
    public float Damage => data.Damage;
    public float MoveSpeed => data.MoveSpeed;

    //Events
    public event Action OnHealthChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHp = data.MaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHp -= amount;
        OnHealthChanged?.Invoke();

         if(CurrentHp <= 0)
         {
            Death();
         }

    }

    void Death()
    {
        IsDead = true;  
        Destroy(gameObject);
    }
}
