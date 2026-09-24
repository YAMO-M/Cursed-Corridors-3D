using System;
using UnityEngine;

public enum GameState { Playing, Won, Lost }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rules")]
    [SerializeField] private int totalCollectibles = 3;
    [SerializeField] private float timeLimit = 300f; // 5 minutes

    public GameState State { get; private set; } = GameState.Playing;
    public bool HasMap { get; private set; }
    public int CollectiblesFound { get; private set; }
    public float TimeRemaining { get; private set; }

    public event Action OnMapCollected;
    public event Action<int, int> OnCollectibleCountChanged;
    public event Action<float> OnTimeChanged;
    public event Action<string> OnGameWon;
    public event Action<string> OnGameLost;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        TimeRemaining = timeLimit;
    }

    void Update()
    {
        if (State != GameState.Playing) return;

        TimeRemaining -= Time.deltaTime;
        OnTimeChanged?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            Lose("Time ran out");
        }
    }

    public void CollectMap()
    {
        if (State != GameState.Playing) return;
        HasMap = true;
        OnMapCollected?.Invoke();
    }

    public void CollectItem()
    {
        if (State != GameState.Playing) return;
        CollectiblesFound++;
        OnCollectibleCountChanged?.Invoke(CollectiblesFound, totalCollectibles);
    }

    public void TryExit()
    {
        if (State != GameState.Playing) return;
        if (!HasMap) return;

        if (CollectiblesFound >= totalCollectibles)
            Win("Perfect Escape");
        else
            Win("Escaped");
    }

    public void Win(string endingName)
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
        Time.timeScale = 0f;
        OnGameWon?.Invoke(endingName);
    }

    public void Lose(string reason)
    {
        if (State != GameState.Playing) return;
        State = GameState.Lost;
        Time.timeScale = 0f;
        OnGameLost?.Invoke(reason);
    }
}