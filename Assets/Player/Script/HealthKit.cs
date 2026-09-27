using UnityEngine;

public class HealthKit : MonoBehaviour, IInteractable
{
    [Header("Health Kit Settings")]
    public int healAmount = 20;

    public string GetPrompt() => "Press M to heal (+" + healAmount + " HP)";

    public void Interact(GameObject interactor)
    {
        PlayerHealth playerHealth = interactor.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }

        gameObject.SetActive(false);
    }
}