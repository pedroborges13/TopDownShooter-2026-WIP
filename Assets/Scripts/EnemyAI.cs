using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;

    [SerializeField] private float updateInterval; //Interval to avoid overloading the CPU

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        InvokeRepeating(nameof(UpdateDestination), 0f, updateInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateDestination()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }
}
