using UnityEngine;

public class MapPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource pickupSound;

    public string GetPrompt() => "Press M to take the Maze Map";

    public void Interact(GameObject interactor)
    {
        GameManager.Instance.CollectMap();
        if (pickupSound != null) pickupSound.Play();
        gameObject.SetActive(false);
    }
}