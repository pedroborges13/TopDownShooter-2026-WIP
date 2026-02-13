using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Chasing, Attacking, Knockback, Dead}
    [Header("State Machine")]
    [SerializeField] private State currentState;

    [Header("References")]
    private EntityStats stats;
    private CharacterAnimationController anim;
    private NavMeshAgent agent;
    private Transform playerTransform;

    [Header("Enemy Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private bool isBoss;
    private bool canAttack;

    [SerializeField] private float friction; //Adjustment for how quiclky the enemy stops
    [SerializeField] private float updateInterval; //Interval to avoid overloading the CPU
    private float pathTimer;
    private GameObject currentBarrierTarget;
    void Start()
    {
        stats = GetComponent<EntityStats>();
        anim = GetComponentInChildren<CharacterAnimationController>();
        agent = GetComponent<NavMeshAgent>();


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (stats != null && agent != null && !isBoss)
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

        canAttack = true;
        SwitchState(State.Chasing);
    }

    void Update()
    {
        if (currentState == State.Dead || stats.IsDead || playerTransform == null) return;

        switch (currentState)
        {
            case State.Chasing:
                HandleMovementLogic();
                break;

            case State.Attacking:
                FaceTarget(); //If attacking: Only rotates to follow the player, then exit. Prevents the enemy from standing like a statue if the player moves around it during the attack
                break;
            case State.Knockback:
                break;

        }
    }

    void SwitchState(State newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case State.Chasing:
                agent.isStopped = false;
                break;
            case State.Attacking:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                break;
            case State.Knockback:
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                break;
            case State.Dead:
                agent.isStopped = true;
                agent.enabled = false;
                break;
        }
    }

    void HandleMovementLogic()
    {
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

        //Debug.Log($"Status: {agent.pathStatus} | Velocity: {agent.velocity.sqrMagnitude}");
    }

    /// <summary>
    /// Logic for when the enemy's path to the player is obstructed
    /// It searches for objects like "Barrier" to destroy them
    /// </summary>
    void HandleBlockedPath()
    {
        //Maintain current target if still valid
        if (currentBarrierTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentBarrierTarget.transform.position);
            if(dist <= attackRange)
            {
                AttemptAttack(currentBarrierTarget);
                return;
            }
            else currentBarrierTarget = null; //Target out of range. Search for new target
        }

        //Detect all coliders within the attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        GameObject bestBarrier = null;
        float lowestHealth = float.MaxValue;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Barrier") && hit.TryGetComponent<BarrierHealth>(out BarrierHealth barrier))
            {
                if (barrier.CurrentHp < lowestHealth)
                {
                lowestHealth = barrier.CurrentHp;   
                bestBarrier = hit.gameObject;
                }
            }
        }

        if (bestBarrier != null ) AttemptAttack(bestBarrier);
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
            FaceTarget(); //Looks at the player before attacking

            AttemptAttack(playerTransform.gameObject);
        }
    }

    /// <summary>
    /// Trigger for the attack. Checks if the cooldown is ready
    /// </summary>
    void AttemptAttack(GameObject target)
    {
        if (canAttack && currentState == State.Chasing)
        {
            StartCoroutine(AttackRoutine(target));
        }
    }

    /// <summary>
    /// Coroutine that handles the attack timing, damage application, and cooldown
    /// </summary>
    IEnumerator AttackRoutine(GameObject target)
    {
        SwitchState(State.Attacking);
        canAttack = false;

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

        yield return new WaitForSeconds(0.5f);

        if (currentState != State.Dead && currentState != State.Knockback)
        {
            SwitchState(State.Chasing);
        }

        float remainingCooldown = attackCooldown - 0.5f;
        if (remainingCooldown > 0) yield return new WaitForSeconds(remainingCooldown);

        canAttack = true;
    }

    /// <summary>
    /// Makes the enemy rotate quickly to face the player
    /// </summary>
    void FaceTarget()
    {
        if (playerTransform == null) return;
        
        Vector3 direction = (playerTransform.position - transform.position).normalized;
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
        if (stats.IsDead) return;

        StopAllCoroutines(); //Reset any ongoing attack or previous knockback
        StartCoroutine(ApplyKnockbackRoutine(initialPosition, force));
    }

    /// <summary>
    /// Coroutine that moves the agent backwards and handles friction
    /// </summary>
    IEnumerator ApplyKnockbackRoutine(Vector3 shotDirection, float force)
    {
        SwitchState(State.Knockback);

        if(agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            SwitchState(State.Chasing);
            yield break; //Stop coroutine
        }

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

        if (!stats.IsDead) SwitchState(State.Chasing);
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
