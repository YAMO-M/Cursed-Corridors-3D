using System.Collections;
using UnityEngine;

// Attach this to any collectible (Map Collectible, HealthKit, etc.).
// Whatever script currently handles pickup logic should call
// TriggerDissolve() instead of destroying the object directly —
// this component handles the visual effect, then destroys it once
// the animation finishes.
public class DissolveOnPickup : MonoBehaviour
{
    [Tooltip("The collectible's Mesh Renderer. Auto-found on Awake if left empty.")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("Drag your M_Dissolve material here.")]
    [SerializeField] private Material dissolveMaterial;

    [Tooltip("How long the dissolve animation takes, in seconds.")]
    [SerializeField] private float dissolveDuration = 1f;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    private bool isDissolving = false;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();
    }

    public void TriggerDissolve()
    {
        if (isDissolving)
            return;

        isDissolving = true;

        // Prevent the player from triggering pickup again mid-animation.
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        // Create a per-instance copy so animating this collectible's
        // dissolve doesn't affect any other object sharing the material.
        Material instanceMat = new Material(dissolveMaterial);
        targetRenderer.material = instanceMat;

        float elapsed = 0f;

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float amount = Mathf.Clamp01(elapsed / dissolveDuration);
            instanceMat.SetFloat(DissolveAmountID, amount);
            yield return null;
        }

        Destroy(gameObject);
    }
}
