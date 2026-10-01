using System.Collections;
using UnityEngine;

public class WallBulbFlicker : MonoBehaviour
{
    [Header("References (auto-found if left empty)")]
    [SerializeField] private Light bulbLight;
    [SerializeField] private Renderer glowRenderer;

    [Header("On/Off Timing (seconds)")]
    [SerializeField] private float minOnTime = 0.6f;
    [SerializeField] private float maxOnTime = 1.75f;
    [SerializeField] private float minOffTime = 0.05f;
    [SerializeField] private float maxOffTime = 0.35f;

    [Header("Stutter (quick flicker before going off)")]
    [Range(0f, 1f)]
    [SerializeField] private float stutterChance = 0.35f;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    private Color baseEmission;
    private Material glowMatInstance;

    private void Awake()
    {
        if (bulbLight == null)
            bulbLight = GetComponentInChildren<Light>();

        if (glowRenderer == null)
        {
            Transform sphere = transform.Find("Sphere");
            if (sphere != null)
                glowRenderer = sphere.GetComponent<Renderer>();
        }

        if (glowRenderer != null)
        {
            glowMatInstance = glowRenderer.material;
            if (glowMatInstance.HasProperty(EmissionColorID))
                baseEmission = glowMatInstance.GetColor(EmissionColorID);
        }
    }

    private void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            SetBulbState(true);
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));

            if (Random.value < stutterChance)
            {
                int stutters = Random.Range(1, 4);
                for (int i = 0; i < stutters; i++)
                {
                    SetBulbState(false);
                    yield return new WaitForSeconds(Random.Range(0.03f, 0.08f));
                    SetBulbState(true);
                    yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
                }
            }

            SetBulbState(false);
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }

    private void SetBulbState(bool on)
    {
        if (bulbLight != null)
            bulbLight.enabled = on;

        if (glowMatInstance != null)
            glowMatInstance.SetColor(EmissionColorID, on ? baseEmission : Color.black);
    }
}