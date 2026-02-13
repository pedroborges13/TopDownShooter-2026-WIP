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

        if (stats != null && agent != null)
        {
            //Random values to give each enemy unique behaviour
            agent.speed = stats.MoveSpeed * Random.Range(0.8f,1.2f); //Apply movement speed from SO/Stats to the NavMeshAgent
            agent.avoidancePriority = Random.Range(30, 70); //Low priority: enemy avoids no one (others avoid it). High priority: enemy avoids everyone to prevent collisions.
            agent.acceleration = agent.acceleration * Random.Range(0.8f, 1.4f); 
            agent.angularSpeed = Random.Range(450, 750); //Rotation speed
            
            float minStop = attackRange * 0.7f; 
            float maxStop = attackRange * 0.9f; 
            agent.stoppingDistance = Random.Range(minStop, maxStop);
        } 
    }

    void Update()
    {
        if (isKnockedback || playerTransform == null || stats.IsDead) return;

        //Debug.Log($"Status: {agent.pathStatus} | Velocity: {agent.velocity.sqrMagnitude}");

        //If attacking: Only rotates to follow the player, then exit
        //Prevents the enemy from standing like a statue if the player moves around it during the attack
        if (isAttacking) 
        {
            FaceTarget();
            return;
        }

        //Optimization: Recalculate the path to the player only at specific intervals
        pathTimer += Time.deltaTime;
        if (pathTimer >= updateInterval)
        {
            agent.SetDestination(playerTransform.position);
            pathTimer = Random.Range(0f, updateInterval);
        }

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
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        //stoppingDistance must be less than attackRange
        if (distanceToPlayer <= agent.stoppingDistance || distanceToPlayer <= attackRange)
        {
            agent.isStopped = true; //Stops movement
            agent.velocity = Vector3.zero; //Resets velocity/inertia to prevent sliding

            FaceTarget(); //Looks at the player before attacking

            AttemptAttack(playerTransform.gameObject);
        }
        else agent.isStopped = false; //If out of range, resume movement 
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
    /// Makes the enemy rotate quickly to face the player
    /// </summary>
    void FaceTarget()
    {
        if (playerTransform != null) return;
        
        Vector3 direction = (playerTransform.position - playerTransform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20); 
        }
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

        //Time for the enemy to recover from being shot before walking again
        yield return new WaitForSeconds(0.2f); 

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
