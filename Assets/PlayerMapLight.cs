using UnityEngine;

public class PlayerMapLight : MonoBehaviour
{
    [SerializeField] private GameObject mapGlow;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.15f;

    [Header("Look")]
    [SerializeField] private Color glowColor = new Color(1f, 0.95f, 0.8f, 1f);
    [SerializeField, Range(0.1f, 3f)] private float intensity = 1f;
    [SerializeField, Range(1f, 4f)] private float falloff = 2f; // higher = tighter core, softer edge

    private Vector3 baseScale;
    private bool active;

    private void Awake()
    {
        baseScale = mapGlow.transform.localScale;

        Renderer r = mapGlow.GetComponent<Renderer>();
        r.material.SetTexture("_BaseMap", CreateGlowTexture(128));
        r.material.SetColor("_BaseColor", glowColor * intensity);

        mapGlow.SetActive(false);
    }

    public void Show()
    {
        active = true;
        mapGlow.SetActive(true);
    }

    private void Update()
    {
        if (!active) return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        mapGlow.transform.localScale = baseScale * Mathf.Lerp(minScale, maxScale, t);
    }

    private Texture2D CreateGlowTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        Vector2 centre = new Vector2(size - 1, size - 1) * 0.5f;
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), centre) / radius;
                float a = Mathf.Pow(Mathf.Clamp01(1f - d), falloff);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        tex.Apply();
        return tex;
    }
}