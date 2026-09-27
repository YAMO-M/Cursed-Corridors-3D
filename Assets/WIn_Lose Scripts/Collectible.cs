using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    public string GetPrompt() => "Press M to collect";

    public void Interact(GameObject interactor)
    {
        GameManager.Instance.CollectItem();
        gameObject.SetActive(false);
    }
}