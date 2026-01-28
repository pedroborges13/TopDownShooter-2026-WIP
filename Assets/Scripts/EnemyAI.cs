using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private EntityStats stats;
    private bool isKnockedback;
    [SerializeField] private float friction; //Adjustment for how quiclky the enemy stops
    [SerializeField] private float updateInterval; //Interval to avoid overloading the CPU

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EntityStats>();

        if (stats != null && agent != null)
        {
            agent.speed = stats.MoveSpeed;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        InvokeRepeating(nameof(UpdateDestination), 0f, updateInterval);
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
}
