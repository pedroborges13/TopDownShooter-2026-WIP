using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private EntityStatsData data;


    //Variáveis locais para permitir modificadores sem alterar o ScriptableObject
    private float maxHp;
    private float moveSpeed;

    public float CurrentHp {  get; private set; }
    public bool IsDead { get; private set; }

    //Getters
    public float MaxHp => maxHp;
    public float Damage => data.Damage;
    public float MoveSpeed => moveSpeed;

    //Events
    public event Action OnHealthChanged;


    void Awake()
    {
        if (data != null)
        {
            maxHp = data.MaxHp;
            moveSpeed = data.MoveSpeed;
            CurrentHp = maxHp;
            Debug.Log("EntityStats: Health: " + maxHp +  " Speed: " + moveSpeed + " CurrentHealth: " + CurrentHp);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupEnemyStats(float hpMod, float speedMod)
    {
        maxHp = data.MaxHp * hpMod;
        moveSpeed = data.MoveSpeed * speedMod;

        //Novo calculo de vida atual
        CurrentHp = maxHp;

        //Atualiza NavMeshAgent
        if(TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.speed = moveSpeed;
        }
    }
    public void TakeDamage(float amount)
    {
        if(IsDead) return;

        CurrentHp -= amount;
        OnHealthChanged?.Invoke();

         if(CurrentHp <= 0)
         {
            if (CompareTag("Enemy"))
            {
                GlobalEvents.OnEnemyKilled?.Invoke();
            }
            Death();
         }

    }

    void Death()
    {
        IsDead = true;  
        Destroy(gameObject);
    }
}
