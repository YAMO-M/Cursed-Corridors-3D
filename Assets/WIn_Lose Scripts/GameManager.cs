using System;
using UnityEngine;

public enum GameState { Playing, Won, Lost }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rules")]
    [SerializeField] private int totalCollectibles = 3;

    public GameState State { get; private set; } = GameState.Playing;
    public bool HasMap { get; private set; }
    public int CollectiblesFound { get; private set; }

    // Other roles subscribe to these instead of polling every frame.
    public event Action OnMapCollected;              // Role 2 reveals the minimap route on this
    public event Action<int, int> OnCollectibleCountChanged;
    public event Action<string> OnGameWon;            // passes the ending name
    public event Action<string> OnGameLost;           // passes the loss reason

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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

    // Called by the exit door trigger.
    public void TryExit()
    {
        if (State != GameState.Playing) return;
        if (!HasMap) return; // door stays locked; the door script shows a prompt

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

    // Role 2's enemy calls this with "Caught by the enemy"
    public void Lose(string reason)
    {
        if (State != GameState.Playing) return;
        State = GameState.Lost;
        Time.timeScale = 0f;
        OnGameLost?.Invoke(reason);
    }
}