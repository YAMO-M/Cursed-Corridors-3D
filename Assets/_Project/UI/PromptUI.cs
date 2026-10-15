using System.Collections;
using TMPro;
using UnityEngine;

public class PromptUI : MonoBehaviour
{
    public static PromptUI Instance { get; private set; }

    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI promptText;
    private Coroutine hideRoutine;

    void Awake()
    {
        Instance = this;
        container.SetActive(false);
    }

    public void Show(string message)
    {
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        promptText.text = message;
        container.SetActive(true);
    }

    public void ShowTimed(string message, float seconds = 3f)
    {
        Show(message);
        hideRoutine = StartCoroutine(HideAfter(seconds));
    }

    public void Hide()
    {
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        container.SetActive(false);
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        container.SetActive(false);
    }
}