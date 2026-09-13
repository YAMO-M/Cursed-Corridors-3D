using UnityEngine;
using UnityEngine.AI;

// Attach this to the zombie prefab alongside a NavMeshAgent and Animator.
// Animator parameters required:
// Float: Speed
// Bool: IsAttacking

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieAI : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Detection")]
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float loseInterestRange = 12f;

    [Header("Movement Speeds")]
    [SerializeField] private float patrolSpeed = 1.2f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;

    private NavMeshAgent agent;
    private Animator animator;

    private State currentState = State.Patrol;

    private int currentPatrolIndex = 0;
    private float lastAttackTime = -999f;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int IsAttackingHash =
        Animator.StringToHash("IsAttacking");


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find the player automatically if one has not been assigned
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }


    private void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:

                Patrol();

                if (distanceToPlayer <= chaseRange)
                {
                    currentState = State.Chase;
                }

                break;


            case State.Chase:

                Chase();

                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attack;
                }
                else if (distanceToPlayer > loseInterestRange)
                {
                    currentState = State.Patrol;
                }

                break;


            case State.Attack:

                Attack();

                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chase;
                }

                break;
        }

        // Update walking/running animation
        float speed = agent.velocity.magnitude;

        animator.SetFloat(SpeedHash, speed);
    }


    private void Patrol()
    {
        agent.speed = patrolSpeed;
        agent.isStopped = false;

        animator.SetBool(IsAttackingHash, false);

        // If no patrol points were assigned,
        // keep the zombie still.
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        Transform target =
            patrolPoints[currentPatrolIndex];

        agent.SetDestination(target.position);

        // Move to the next patrol point
        if (!agent.pathPending &&
            agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }
        }
    }


    private void Chase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;

        animator.SetBool(IsAttackingHash, false);

        // Follow the player
        agent.SetDestination(player.position);
    }


    private void Attack()
    {
        agent.isStopped = true;

        // Make zombie face the player
        Vector3 lookPosition = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );

        transform.LookAt(lookPosition);


        // Attack after cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Play attack animation
            animator.SetBool(IsAttackingHash, true);

            // Damage player
            PlayerHealth health =
                player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
        else
        {
            animator.SetBool(IsAttackingHash, false);
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            chaseRange
        );

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}