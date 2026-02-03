using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator anim;
    private EntityStats stats;
    private CharacterController controller;
    private NavMeshAgent agent;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<EntityStats>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (stats != null && stats.IsDead) return;

        float currentSpeed = 0;
        if (agent != null) currentSpeed = agent.velocity.magnitude;
        else if (controller != null) currentSpeed = controller.velocity.magnitude;

        anim.SetFloat("Speed", currentSpeed);
    }

    public void PlayAttack()
    {
        if (stats.IsDead) return;

        anim.SetTrigger("Attack");
    }

    public void PlayHit()
    {
        if (stats.IsDead) return;

        anim.SetTrigger("Hit");
    }

    public void PlayDeath()
    {
        anim.SetTrigger("Dead");
    }
}
