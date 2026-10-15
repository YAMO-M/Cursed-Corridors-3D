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
        Attacking,
        Returning
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Renderer detectionConeRenderer;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float loseRange = 15f;
    [SerializeField] private float closeRange = 1.5f; // arm's length - always triggers chase, ignores line of sight
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float eyeHeight = 1.5f;

    [Header("Movement")]
    [SerializeField] private float updateInterval = 0.2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f; // keep >= NavMeshAgent Stopping Distance or the zombie will never reach this range
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Return To Spawn")]
    [SerializeField] private float arrivalThreshold = 0.3f;

    private NavMeshAgent agent;
    private State currentState = State.Idle;

    private float timer;
    private float attackTimer;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private static readonly int AlertColorID = Shader.PropertyToID("_AlertColor");

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

        // Remember where this zombie started so it can walk back here
        // once it loses the player.
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

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

            case State.Returning:
                HandleReturning(distance, canSeePlayer);
                break;
        }

        UpdateAnimation();
    }

    private void SetState(State newState)
    {
        if (currentState == newState)
            return;
        currentState = newState;
        Debug.Log("Zombie State:" + newState);

        if (detectionConeRenderer != null)
        {
            // Idle and Returning both read as "not actively hunting" -> amber.
            // Chasing and Attacking read as alerted -> red.
            bool alerted = currentState == State.Chasing || currentState == State.Attacking;
            Color alertColor = alerted ? Color.red : Color.yellow;
            detectionConeRenderer.material.SetColor(AlertColorID, alertColor);
        }
    }

    private void HandleIdle(float distance, bool canSeePlayer)
    {
        agent.isStopped = true;

        bool closeEnoughRegardless = distance <= closeRange;
        bool spottedAtRange = distance <= detectionRange && canSeePlayer;

        if (closeEnoughRegardless || spottedAtRange)
        {
            SetState(State.Chasing);
            agent.isStopped = false;
        }
    }

    private void HandleChasing(float distance, bool canSeePlayer)
    {
        if (distance > loseRange || !canSeePlayer)
        {
            StartReturning();
            return;
        }

        // Only attack when genuinely close
        if (distance <= attackRange)
        {
            SetState(State.Attacking);
            agent.isStopped = true;
            agent.ResetPath();
            attackTimer = 0f;
            return;
        }

        // Keep walking toward the player
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

        // Player moved away, chase again
        if (distance > attackRange)
        {
            SetState(State.Chasing);
            agent.isStopped = false;
            return;
        }

        // Count down to the next attack
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack");

            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);

            attackTimer = attackCooldown;
        }
    }

    private void StartReturning()
    {
        SetState(State.Returning);
        agent.isStopped = false;
        agent.SetDestination(spawnPosition);
    }

    private void HandleReturning(float distance, bool canSeePlayer)
    {
        // Player came back into range on the way home - chase again.
        bool closeEnoughRegardless = distance <= closeRange;
        bool spottedAtRange = distance <= detectionRange && canSeePlayer;

        if (closeEnoughRegardless || spottedAtRange)
        {
            SetState(State.Chasing);
            return;
        }

        float distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);

        if (!agent.pathPending && distanceToSpawn <= arrivalThreshold)
        {
            SetState(State.Idle);
            agent.isStopped = true;
            agent.ResetPath();
            transform.rotation = spawnRotation;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, closeRange);

        Gizmos.color = Color.cyan;
        Vector3 spawnGizmoPos = Application.isPlaying ? spawnPosition : transform.position;
        Gizmos.DrawWireSphere(spawnGizmoPos, 0.5f);
    }
}