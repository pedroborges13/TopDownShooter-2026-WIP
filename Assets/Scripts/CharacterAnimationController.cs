using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator anim;
    private EntityStats stats;

    private Vector3 lastPosition;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<EntityStats>();

    }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (stats != null && stats.IsDead) return;
        if (Time.deltaTime <= Mathf.Epsilon) return; //Required check: Time.deltTime is 0 when paused, which breaks run animation calculations

        //Get the current position
        Vector3 currentPosition = transform.position;

        //Calculate distance travaled
        Vector3 posNow = new Vector3(currentPosition.x, 0, currentPosition.z);
        Vector3 posLast = new Vector3(lastPosition.x, 0, lastPosition.z);

        float distanceTraveled = Vector3.Distance(posNow, posLast);

        //Real speed
        float currentRealSpeed = distanceTraveled / Time.deltaTime;

        //Immediatly zero out the speed if it's below a threshold to prevent sliding
        if (currentRealSpeed < 0.1f) currentRealSpeed = 0;

        //Send value to Animator using DampTime. The third paramater (0.1f) handles the interpolation for smooth transitions (Decrease it for even faster response)
        anim.SetFloat("Speed", currentRealSpeed, 0.1f, Time.deltaTime);

        //Update last position for the next frame
        lastPosition = currentPosition;
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
