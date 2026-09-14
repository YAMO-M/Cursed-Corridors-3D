using UnityEngine;

public class HealthKit : MonoBehaviour
{
    [Header("Health Kit Settings")]
    public int healAmount = 20;

    private bool playerInRange = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found!");
        }
    }

    void Update()
    {
        // Check if player is in range AND pressed M
        if (playerInRange && Input.GetKeyDown(KeyCode.M))
        {
            CollectHealthKit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("💊 Press M to collect the Health Kit!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("❌ Left the Health Kit area");
        }
    }

    void CollectHealthKit()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log("❤️ Health Kit collected! +" + healAmount + " HP");
        }

        Destroy(gameObject);
    }
}