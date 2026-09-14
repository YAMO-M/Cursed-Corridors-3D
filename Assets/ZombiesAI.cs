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
    [SerializeField] private float attackRange = 1.8f;
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
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = true;

        animator.SetFloat("Speed", 0f);
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        bool canSeePlayer =
            HasLineOfSight(distance);

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

        UpdateAnimation();
    }

    private void HandleIdle(float distance, bool canSeePlayer)
    {
        agent.isStopped = true;

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

        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            agent.SetDestination(player.position);
        }

        if (distance <= attackRange &&
            attackTimer <= 0f)
        {
            currentState = State.Attacking;
            agent.isStopped = true;

            animator.SetTrigger("Attack");

            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);

            attackTimer = attackCooldown;
        }

        attackTimer -= Time.deltaTime;
    }

    private void HandleAttacking(float distance)
    {
        agent.isStopped = true;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (distance <= attackRange)
            {
                animator.SetTrigger("Attack");
                attackTimer = attackCooldown;
            }
            else
            {
                currentState = State.Chasing;
                agent.isStopped = false;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        // Get the zombie's actual movement speed
        float movementSpeed = agent.velocity.magnitude;

        // Send the speed to the Animator
        animator.SetFloat("Speed", movementSpeed);
    }

    private bool HasLineOfSight(float distance)
    {
        Vector3 origin =
            transform.position + Vector3.up * eyeHeight;

        Vector3 target =
            player.position + Vector3.up * eyeHeight;

        Vector3 direction =
            (target - origin).normalized;

        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            distance,
            obstacleMask))
        {
            return false;
        }

        return true;
    }
}