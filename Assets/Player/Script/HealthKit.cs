using UnityEngine;

[RequireComponent(typeof(DissolveOnPickup))]
public class HealthKit : MonoBehaviour
{
    [Header("Health Kit Settings")]
    public int healAmount = 20;

    private bool playerInRange = false;
    private PlayerHealth playerHealth;
    private DissolveOnPickup dissolveOnPickup;

    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        dissolveOnPickup = GetComponent<DissolveOnPickup>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found!");
        }
    }

    void Update()
    {
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

        dissolveOnPickup.TriggerDissolve();
    }
}