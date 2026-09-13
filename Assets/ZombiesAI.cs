using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieChaseAI : MonoBehaviour
{
    private enum State { Idle, Chasing }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;    // max distance the zombie can spot the player at all
    [SerializeField] private float loseRange = 15f;         // if player gets this far, zombie gives up and idles again
    [SerializeField] private LayerMask obstacleMask;        // set this to your Walls/Maze layer - blocks line of sight
    [SerializeField] private float eyeHeight = 1.5f;        // raycast origin height, roughly zombie head height

    [Header("Chase Settings")]
    [SerializeField] private float updateInterval = 0.2f;   // how often to recalc path (perf-friendly for 12 zombies)

    private NavMeshAgent agent;
    private State currentState = State.Idle;
    private float timer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        agent.isStopped = true; // start idle - don't move until the player is spotted
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = HasLineOfSight(distanceToPlayer);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange && canSeePlayer)
                {
                    currentState = State.Chasing;
                    agent.isStopped = false;
                }
                break;

            case State.Chasing:
                if (distanceToPlayer > loseRange || !canSeePlayer)
                {
                    currentState = State.Idle;
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer >= updateInterval)
                    {
                        timer = 0f;
                        agent.SetDestination(player.position);
                    }
                }
                break;
        }

        // Optional: drive a Speed float once you add an Idle/Walk blend tree in the Animator
        // if (animator != null)
        // {
        //     float speedPercent = agent.velocity.magnitude / agent.speed;
        //     animator.SetFloat("Speed", speedPercent);
        // }
    }

    // Returns true only if nothing on obstacleMask (walls) sits between zombie and player -
    // this is what stops the zombie "seeing" the player through maze walls.
    private bool HasLineOfSight(float distanceToPlayer)
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * eyeHeight;
        Vector3 direction = (target - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distanceToPlayer, obstacleMask))
        {
            // Something on the wall layer is blocking the view
            return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        if (player != null)
        {
            Gizmos.color = HasLineOfSight(Vector3.Distance(transform.position, player.position)) ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up * eyeHeight, player.position + Vector3.up * eyeHeight);
        }
    }
}