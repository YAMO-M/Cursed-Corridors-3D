using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winTitleText;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text loseTitleText;
    [SerializeField] private TMP_Text loseDetailText;

    void Start()
    {
        GameManager.Instance.OnGameWon += HandleWin;
        GameManager.Instance.OnGameLost += HandleLoss;
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnGameWon -= HandleWin;
        GameManager.Instance.OnGameLost -= HandleLoss;
    }

    private void HandleWin(string endingName)
    {
        winPanel.SetActive(true);
        winTitleText.text = endingName == "Perfect Escape" ? "PERFECT ESCAPE!" : "YOU WIN!";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleLoss(string reason)
    {
            losePanel.SetActive(true);

        if (reason == "Time ran out")
            loseTitleText.text = "TIME'S UP!";
        else
            loseTitleText.text = "CAUGHT!";

        loseDetailText.text = reason;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}