using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 10f;      // How far the zombie can see you
    public float attackRange = 1.8f;    // How close to attack
    public float attackCooldown = 2f;   // Wait between attacks
    public int damage = 10;             // Damage per attack

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the player automatically
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No Player found! Make sure your player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- STATE 1: ATTACK ---
        if (distance <= attackRange)
        {
            // Stop moving
            agent.ResetPath();

            // Attack with cooldown
            if (Time.time > lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackPlayer();
            }
        }

        // --- STATE 2: CHASE ---
        else if (distance <= chaseRange)
        {
            agent.SetDestination(player.position);
        }

        // --- STATE 3: IDLE ---
        else
        {
            agent.ResetPath();
        }
    }

    void AttackPlayer()
    {
        // Find the player's health script and damage them
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("🧟 Zombie attacked! Health: " + health.currentHealth);
        }
    }
}