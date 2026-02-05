using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private EntityStats stats;
    private CharacterAnimationController anim;
    private NavMeshAgent agent;
    private Transform playerTransform;

    [Header("Enemy Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    private bool isAttacking;

    [SerializeField] private float friction; //Adjustment for how quiclky the enemy stops
    [SerializeField] private float updateInterval; //Interval to avoid overloading the CPU
    private float pathTimer;

    //State
    private bool isKnockedback;
    void Start()
    {
        stats = GetComponent<EntityStats>();
        anim = GetComponentInChildren<CharacterAnimationController>();
        agent = GetComponent<NavMeshAgent>();


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        //Apply movement speed from SO/Stats to the NavMeshAgent
        if (stats != null && agent != null)
        {
            agent.speed = stats.MoveSpeed;
        } 
    }

    void Update()
    {
        if (isKnockedback || playerTransform == null || stats.IsDead) return;

        //Debug.Log($"Status: {agent.pathStatus} | Velocity: {agent.velocity.sqrMagnitude}");

        //Optimization: Recalculate the path to the player only at specific intervals
        pathTimer += Time.deltaTime;
        if (pathTimer >= updateInterval)
        {
            agent.SetDestination(playerTransform.position);
            pathTimer = 0;
        }

        //If currently performing an attack animation/routine, skip movement logic
        if (isAttacking) return;

        //Path Status logic:
        //Patial Path means the NavMesh is blocked (likely by a Barrier)
        if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            HandleBlockedPath();
        }
        else
        {
            HandleChasePath();
        }
    }

    /// <summary>
    /// Logic for when the enemy's path to the player is obstructed
    /// It searches for objects like "Barrier" to destroy them
    /// </summary>
    void HandleBlockedPath()
    {
        Debug.Log("Caminho bloqueado, procurando barrier");

        //Detect all coliders within the attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        bool barrierFound = false;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Barrier"))
            {
            //Stop the agent to attack
            agent.isStopped = true;
            AttemptAttack(hit.gameObject);
            barrierFound = true;
            Debug.Log("Atacou a Barrier");
            break; //Focus on one barrier at a time
            }
        }

        //If no barries is nearby, resume movement
        if (!barrierFound)
        {
        agent.isStopped = false;
        }
    }

    /// <summary>
    /// Logic for when the path is clear
    /// The enemy moves toward the player and attacks when within stopping distance
    /// </summary>
    void HandleChasePath()
    {
        agent.isStopped = false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        //stoppingDistance tem que ser menor que o attack range
        if (distanceToPlayer <= agent.stoppingDistance)
        {
            AttemptAttack(playerTransform.gameObject);
        }
    }

    /// <summary>
    /// Trigger for the attack. Checks if the cooldown is ready
    /// </summary>
    void AttemptAttack(GameObject target)
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine(target));
        }
    }

    /// <summary>
    /// Coroutine that handles the attack timing, damage application, and cooldown
    /// </summary>
    IEnumerator AttackRoutine(GameObject target)
    {
        isAttacking = true;
        agent.isStopped = true;

        if (target != null)
        {
            BarrierHealth targetHealth = target.GetComponent<BarrierHealth>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(stats.Damage);
                Debug.Log("HIT BARRIER!");
                anim.PlayAttack();
            }
            else
            {
                EntityStats playerStats = target.GetComponent<EntityStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(stats.Damage);
                    Debug.Log("HIT PLAYER!");
                    anim.PlayAttack();
                }
            }
            
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        agent.isStopped = false; //Resume movement
    }

    /// <summary>
    /// Public method to trigger a knocback effect from projectiles or explosions
    /// </summary>
    public void ApplyKnockback(Vector3 initialPosition, float force)
    {
        if (gameObject.activeInHierarchy) //If the enemy has died
        {
            StopAllCoroutines(); //Reset any ongoing attack or previous knockback
            StartCoroutine(ApplyKnockbackRoutine(initialPosition, force));
        }
    }

    /// <summary>
    /// Coroutine that moves the agent backwards and handles friction
    /// </summary>
    IEnumerator ApplyKnockbackRoutine(Vector3 shotDirection, float force)
    {
        isKnockedback = true;

        if(agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            isKnockedback = false;
            yield break; //Stop coroutine
        }

        agent.isStopped = true; //Stops persuits
        Vector3 direction = shotDirection.normalized;
        direction.y = 0; //Ensures knockback is only horizontal

        float currentForce = force;

        //Gradually reduce the force over time until it's insignificant
        while (currentForce > 0.2f)
        {
            if(agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.Move(direction * currentForce * Time.deltaTime);
            }
            currentForce = Mathf.Lerp(currentForce, 0, friction * Time.deltaTime);
            yield return null;
        }

        isKnockedback = false;
        if(agent != null && agent.enabled) agent.isStopped = false; 
    }

    /// <summary>
    /// Visualizes the attack range in the Unity Editor for easier debugging
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (agent != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
        }
    }
}
