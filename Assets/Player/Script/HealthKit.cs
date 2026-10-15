using UnityEngine;

[RequireComponent(typeof(DissolveOnPickup))]
public class HealthKit : MonoBehaviour
{
    [Header("Health Kit Settings")]
    public int healAmount = 20;

    private bool playerInRange = false;
    private bool collected = false;
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
        if (playerInRange && !collected && Input.GetKeyDown(KeyCode.M))
        {
            CollectHealthKit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (PromptUI.Instance != null)
                PromptUI.Instance.Show("Press M to collect the Health Kit");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!collected && PromptUI.Instance != null)
                PromptUI.Instance.Hide();
        }
    }

    void CollectHealthKit()
    {
        collected = true;
        playerInRange = false;

        if (PromptUI.Instance != null)
            PromptUI.Instance.ShowTimed("Health Kit collected! +" + healAmount + " HP", 2f);

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }

        dissolveOnPickup.TriggerDissolve();
    }
}