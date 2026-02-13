using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private bool canReceiveKnockback;
    [SerializeField] private EntityStatsData data;

    //References
    private CharacterAnimationController anim;
    private NavMeshAgent agent;
    private CharacterController playerController;

    //Local variables allow modifications without altering the ScriptableObject
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
        if (CompareTag("Enemy")) agent = GetComponent<NavMeshAgent>();
        else if (CompareTag("Player")) playerController = GetComponent<CharacterController>();
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

        if (kbForce > 0 && TryGetComponent<EnemyAI>(out EnemyAI ai))
        {
            if(canReceiveKnockback) ai.ApplyKnockback(initialPosition, kbForce);
        }

        if (CurrentHp > 0 && canReceiveKnockback) anim.PlayHit();

        if (CompareTag("Player")) OnHealthChanged?.Invoke(); //Notifies the UIManager

        if (CurrentHp <= 0)
        {
           anim.PlayDeath();

           if (CompareTag("Enemy"))
           {
               GlobalEvents.OnEnemyKilled?.Invoke();
               DisableNavMesh();

               if (TryGetComponent<EnemyDrop>(out EnemyDrop drop))
               {
                   drop.DropReward();
               }   
               if (TryGetComponent<BoxCollider>(out BoxCollider collider))
               {
                    collider.enabled = false;
               }
           }
           Death();
         }
    }

    void DisableNavMesh()
    {
        if(agent != null)
        {
            agent.speed = 0;
            agent.isStopped = true;
            agent.enabled = false;
        }
    }
    void Death()
    {
        IsDead = true;  
        if(CompareTag("Player")) playerController.enabled = false;
        Destroy(gameObject,1);
    }
}
