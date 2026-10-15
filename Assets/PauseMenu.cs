using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string startSceneName = "StartScene";

    public static bool IsPaused { get; private set; }

    void Start() { SetPaused(false); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // don't open pause once the game has ended
            if (Time.timeScale == 0f && !IsPaused) return;
            SetPaused(!IsPaused);
        }
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void Resume() { SetPaused(false); }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(startSceneName);
    }

    public void QuitGame() { Application.Quit(); }
}