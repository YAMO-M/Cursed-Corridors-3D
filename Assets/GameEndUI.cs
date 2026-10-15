using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private string startSceneName = "StartScene";

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        GameManager.Instance.OnGameWon += HandleWin;
        GameManager.Instance.OnGameLost += HandleLose;
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnGameWon -= HandleWin;
        GameManager.Instance.OnGameLost -= HandleLose;
    }

    void HandleWin(string msg) { Show(winPanel); }
    void HandleLose(string msg) { Show(losePanel); }

    void Show(GameObject panel)
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame() { Application.Quit(); }
}