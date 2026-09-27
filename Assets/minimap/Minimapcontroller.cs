using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject minimapPanel;

    [Header("Fog Reveal")]
    [SerializeField] private List<GameObject> fogPieces = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private float displayDuration = 15f;

    private int mapPiecesCollected = 0;
    private Coroutine hideCoroutine;

    public bool HasAnyMap => mapPiecesCollected > 0;

    public bool HasFullMap =>
        fogPieces.Count == 0 ||
        mapPiecesCollected >= fogPieces.Count;

    public int PiecesCollected => mapPiecesCollected;

    private void Awake()
    {
        mapPiecesCollected = 0;

        // Hide minimap when the game starts.
        if (minimapPanel != null)
        {
            minimapPanel.SetActive(false);
        }

        // Keep all fog pieces covering the map.
        foreach (GameObject fog in fogPieces)
        {
            if (fog != null)
            {
                fog.SetActive(true);
            }
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapCollected += OnMapCollected;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMapCollected -= OnMapCollected;
        }
    }

    private void OnMapCollected()
    {
        // If the minimap is already showing,
        // hide the old version first.
        if (minimapPanel != null)
        {
            minimapPanel.SetActive(false);
        }

        // Cancel the previous 15-second timer.
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Reveal the next section.
        RevealNextMapSection();

        // Show the updated map.
        if (minimapPanel != null)
        {
            minimapPanel.SetActive(true);
        }

        // Start a new 15-second timer.
        hideCoroutine = StartCoroutine(HideAfter15Seconds());
    }

    private void RevealNextMapSection()
    {
        if (mapPiecesCollected >= fogPieces.Count)
        {
            return;
        }

        GameObject fogToRemove = fogPieces[mapPiecesCollected];

        if (fogToRemove != null)
        {
            fogToRemove.SetActive(false);
        }

        mapPiecesCollected++;
    }

    private IEnumerator HideAfter15Seconds()
    {
        yield return new WaitForSeconds(15f);

        if (minimapPanel != null)
        {
            minimapPanel.SetActive(false);
        }

        hideCoroutine = null;
    }
}