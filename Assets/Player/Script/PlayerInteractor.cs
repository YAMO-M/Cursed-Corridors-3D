using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    private IInteractable currentInteractable;

    void Start()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        // Listen for M
        if (Input.GetKeyDown(KeyCode.M) && currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
            currentInteractable = null;

            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;

            if (promptPanel != null)
                promptPanel.SetActive(true);

            if (promptText != null)
                promptText.text = interactable.GetPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;

            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }
}