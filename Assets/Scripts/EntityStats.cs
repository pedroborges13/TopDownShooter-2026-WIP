using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private EntityStatsData data;
    private CharacterAnimationController anim;

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
            //Debug.Log("EntityStats: Health: " + maxHp +  " Speed: " + moveSpeed + " CurrentHealth: " + CurrentHp);
        }
    }

    void Start()
    {
        anim = GetComponent<CharacterAnimationController>();
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
    public void TakeDamage(float damage, Vector3 initialPosition = default, float kbForce = 0) //Default and 0 for when the player takes damage
    {
        if(IsDead) return;

        CurrentHp -= damage;

        if (CurrentHp > 0 && CompareTag("Enemy")) anim.PlayHit();

        if (CompareTag("Player")) OnHealthChanged?.Invoke(); //Notifies the UIManager

        if(kbForce > 0 && TryGetComponent<EnemyAI>(out EnemyAI ai))
        {
            ai.ApplyKnockback(initialPosition, kbForce);
        }

         if (CurrentHp <= 0)
         {
            anim.PlayDeath();

            if (CompareTag("Enemy"))
            {
                GlobalEvents.OnEnemyKilled?.Invoke();

                if (TryGetComponent<EnemyDrop>(out EnemyDrop drop))
                {
                    drop.DropReward();
                }
            }
            Death();
         }
    }

    void Death()
    {
        IsDead = true;  
        Destroy(gameObject,1);
    }
}
