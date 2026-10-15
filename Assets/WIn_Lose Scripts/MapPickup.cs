using UnityEngine;

public class MapPickup : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource pickupSound;

    private bool collected = false;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && !collected && Input.GetKeyDown(KeyCode.M))
        {
            CollectMap();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (PromptUI.Instance != null)
                PromptUI.Instance.Show("Press M to collect the Map");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!collected && PromptUI.Instance != null)
                PromptUI.Instance.Hide();
        }
    }

    private void CollectMap()
    {
        if (collected)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("MapPickup: GameManager.Instance could not be found.");
            return;
        }

        collected = true;
        playerInRange = false;

        // Show the map-only glow on the player
        FindAnyObjectByType<PlayerMapLight>()?.Show();
        GameManager.Instance.CollectMap();
        GameManager.Instance.CollectItem();

        if (pickupSound != null)
        {
            pickupSound.Play();
        }

        if (PromptUI.Instance != null)
            PromptUI.Instance.ShowTimed("Map collected! The minimap is now showing", 3f);

        Debug.Log("Maze map collected!");

        gameObject.SetActive(false);
    }
}