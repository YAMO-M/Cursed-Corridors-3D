using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject minimapPanel;

    [Header("Fog Reveal")]
    [Tooltip("Fog pieces are revealed in the same order that maps are collected.")]
    [SerializeField] private List<GameObject> fogPieces = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private float displayDuration = 30f;

    private float timer;
    private bool isShowing;
    private int mapPiecesCollected;

    public bool HasAnyMap => mapPiecesCollected > 0;

    public bool HasFullMap =>
        fogPieces.Count == 0 ||
        mapPiecesCollected >= fogPieces.Count;

    public int PiecesCollected => mapPiecesCollected;

    private void Awake()
    {
        mapPiecesCollected = 0;
        isShowing = false;
        timer = 0f;

        // Minimap starts hidden.
        if (minimapPanel != null)
            minimapPanel.SetActive(false);

        // All fog starts covering the minimap.
        foreach (GameObject fog in fogPieces)
        {
            if (fog != null)
                fog.SetActive(true);
        }
    }

    private void Start()
    {
        // Subscribe after all Awake methods have run.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapCollected += OnMapCollected;
        }
        else
        {
            Debug.LogError(
                "MinimapController: GameManager.Instance could not be found."
            );
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapCollected -= OnMapCollected;
        }
    }

    private void Update()
    {
        if (!isShowing)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            HideMinimap();
        }
    }

    private void OnMapCollected()
    {
        RevealNextMapSection();
        ShowMinimap();
    }

    private void RevealNextMapSection()
    {
        // Prevent revealing more fog than actually exists.
        if (mapPiecesCollected >= fogPieces.Count)
        {
            Debug.Log("All map sections have already been revealed.");
            return;
        }

        GameObject fogToRemove = fogPieces[mapPiecesCollected];

        if (fogToRemove != null)
        {
            fogToRemove.SetActive(false);
        }

        mapPiecesCollected++;

        Debug.Log(
            $"Map section revealed: " +
            $"{mapPiecesCollected}/{fogPieces.Count}"
        );
    }

    private void ShowMinimap()
    {
        isShowing = true;

        // Every newly collected map gives the player another
        // 30 seconds of minimap visibility.
        timer = displayDuration;

        if (minimapPanel != null)
        {
            minimapPanel.SetActive(true);
        }
    }

    private void HideMinimap()
    {
        isShowing = false;
        timer = 0f;

        if (minimapPanel != null)
        {
            minimapPanel.SetActive(false);
        }
    }
}