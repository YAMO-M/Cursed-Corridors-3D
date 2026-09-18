using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieChaseAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        Chasing,
        Attacking
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float loseRange = 15f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float eyeHeight = 1.5f;

    [Header("Movement")]
    [SerializeField] private float updateInterval = 0.2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.8f; // keep >= NavMeshAgent Stopping Distance
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private State currentState = State.Idle;

    private float timer;
    private float attackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        // IMPORTANT: Apply Root Motion must be CHECKED on the Animator component.
        // The Walk animation itself moves the character; the NavMeshAgent only
        // handles pathfinding direction, not the actual translation. This is
        // what keeps the legs' movement and the body's movement in sync.
        agent.updatePosition = false;
        agent.updateRotation = true;
        agent.isStopped = true;

        animator.SetBool("IsChasing", false);
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = HasLineOfSight(distance);

        switch (currentState)
        {
            case State.Idle:
                HandleIdle(distance, canSeePlayer);
                break;

            case State.Chasing:
                HandleChasing(distance, canSeePlayer);
                break;

            case State.Attacking:
                HandleAttacking(distance);
                break;
        }
    }

    // Called automatically by Unity whenever the Animator applies root motion.
    // We take exactly how far the WALK animation moved this frame and apply
    // that same amount to both the transform and the NavMeshAgent, so the legs
    // and the body always move the identical distance - no more sliding.
    private void OnAnimatorMove()
    {
        if (currentState != State.Chasing)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Zombie Walk")) // match your exact state name in the Animator graph
            return;

        Vector3 newPosition = transform.position + animator.deltaPosition;
        agent.nextPosition = newPosition;
        transform.position = agent.nextPosition;
    }

    private void HandleIdle(float distance, bool canSeePlayer)
    {
        agent.isStopped = true;
        animator.SetBool("IsChasing", false);

        if (distance <= detectionRange && canSeePlayer)
        {
            currentState = State.Chasing;
            agent.isStopped = false;
        }
    }

    private void HandleChasing(float distance, bool canSeePlayer)
    {
        if (distance > loseRange || !canSeePlayer)
        {
            currentState = State.Idle;
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        if (distance <= attackRange)
        {
            currentState = State.Attacking;
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetBool("IsChasing", false);
            attackTimer = 0f;
            return;
        }

        animator.SetBool("IsChasing", true);

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private void HandleAttacking(float distance)
    {
        agent.isStopped = true;
        animator.SetBool("IsChasing", false);

        if (distance > attackRange)
        {
            currentState = State.Chasing;
            agent.isStopped = false;
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack");

            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);

            attackTimer = attackCooldown;
        }
    }

    private bool HasLineOfSight(float distance)
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * eyeHeight;
        Vector3 direction = (target - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleMask))
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}