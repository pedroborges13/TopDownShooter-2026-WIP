using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent agent;
    private Transform playerTransform;
    private EntityStats stats;

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
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EntityStats>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        //Stats do SO
        if (stats != null && agent != null)
        {
            agent.speed = stats.MoveSpeed;
        } 
    }

    void Update()
    {
        if (isKnockedback || playerTransform != null) return;

        //Timer para nao calcular rota todo update{
        pathTimer += Time.deltaTime;
        if (pathTimer >= updateInterval)
        {
            agent.SetDestination(playerTransform.position);
            pathTimer = 0;
        }

        if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            HandleBlockedPath();
        }
        else
        {
            HandleChasePath();
        }
    }

    void HandleBlockedPath()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        bool barrierFound = false;

        foreach (var hit in hitColliders)
        {
            if (CompareTag("Barrier"))
            {
                //Para de andar para bater
                agent.isStopped = true;
                AttemptAttack(hit.gameObject);
                barrierFound = true;
                break; //Foca em uma barreira por vez
            }
        }

        if (!barrierFound)
        {
            agent.isStopped = false;
        }
    }

    void HandleChasePath()
    {
        agent.isStopped = false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= agent.stoppingDistance)
        {
            AttemptAttack(playerTransform.gameObject);
        }
    }

    void AttemptAttack(GameObject target)
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine(target));
        }
    }

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
                //anim.SetTrigger("Attack")
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        agent.isStopped = false;
    }

    public void ApplyKnockback(Vector3 initialPosition, float force)
    {
        if (gameObject.activeInHierarchy) //If the enemy has died
        {
            StopAllCoroutines(); //Interrupts the previous knockback
            StartCoroutine(ApplyKnockbackRoutine(initialPosition, force));
        }
    }

    IEnumerator ApplyKnockbackRoutine(Vector3 shotDirection, float force)
    {
        isKnockedback = true;
        agent.isStopped = true; //Stops persuits

        Vector3 direction = shotDirection.normalized;
        direction.y = 0; //Ensures knockback is only horizontal

        float currentForce = force;

        while (currentForce > 0.2f)
        {
            agent.Move(direction * currentForce * Time.deltaTime);
            currentForce = Mathf.Lerp(currentForce, 0, friction * Time.deltaTime);

            yield return null;
        }

        isKnockedback = false;
        agent.isStopped = false; 
    }

    void UpdateDestination()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
