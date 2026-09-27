
using UnityEngine;

public class MapPickup : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource pickupSound;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only collect when the Player walks into the map.
        if (!other.CompareTag("Player"))
            return;

        CollectMap();
    }

    private void CollectMap()
    {
        // Prevent collecting the same map more than once.
        if (collected)
            return;

        // Make sure the GameManager exists.
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "MapPickup: GameManager.Instance could not be found."
            );

            return;
        }

        collected = true;

        // Tell the GameManager that a map was collected.
        // MinimapController listens for this event.
        GameManager.Instance.CollectMap();

        // Play pickup sound.
        if (pickupSound != null)
        {
            pickupSound.Play();
        }

        Debug.Log("Maze map collected!");

        // Remove the map collectible.
        gameObject.SetActive(false);
    }
}