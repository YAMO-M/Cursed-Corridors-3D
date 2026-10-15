using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MinimapHideBulbLights : MonoBehaviour
{
    [SerializeField] private string bulbLayerName = "Bulbs";

    private Camera cam;
    private readonly List<Light> bulbLights = new List<Light>();
    private readonly List<bool> wasEnabled = new List<bool>();

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        int layer = LayerMask.NameToLayer(bulbLayerName);
        foreach (Light l in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (l.gameObject.layer == layer)
                bulbLights.Add(l);
        }
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBegin;
        RenderPipelineManager.endCameraRendering += OnEnd;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBegin;
        RenderPipelineManager.endCameraRendering -= OnEnd;
    }

    private void OnBegin(ScriptableRenderContext ctx, Camera c)
    {
        if (c != cam) return;

        wasEnabled.Clear();
        foreach (Light l in bulbLights)
        {
            wasEnabled.Add(l != null && l.enabled);
            if (l != null) l.enabled = false;
        }
    }

    private void OnEnd(ScriptableRenderContext ctx, Camera c)
    {
        if (c != cam) return;

        for (int i = 0; i < bulbLights.Count; i++)
        {
            if (bulbLights[i] != null)
                bulbLights[i].enabled = wasEnabled[i];
        }
    }
}
