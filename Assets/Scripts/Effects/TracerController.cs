using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TracerController : MonoBehaviour
{
    [Header("Tracer Settings")]
    public Color tracerColor = Color.yellow;
    public float lineWidth = 0.05f;
    public float fadeDuration = 0.2f;

    private ParticleSystem ps;
    private LineRenderer lineRenderer;
    private Material lineMaterial;
    private float timer;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        CreateLineRenderer();
    }

    private void CreateLineRenderer()
    {
        // Create or get existing LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Use a pre-made material that's included in Resources
        lineMaterial = Instantiate(Resources.Load<Material>("TracerMaterial"));
        lineMaterial.color = tracerColor;
        lineRenderer.material = lineMaterial;

        // Configure line properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startColor = tracerColor;
        lineRenderer.endColor = tracerColor;
    }

    public void Initialize(Vector3 startPos, Vector3 endPos, float tracerDuration)
    {
        // Set line positions
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Configure particle system
        var main = ps.main;
        main.startLifetime = tracerDuration;
        transform.position = endPos;
        ps.Play();

        timer = 0f;
        Destroy(gameObject, tracerDuration + fadeDuration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > fadeDuration)
        {
            float progress = (timer - fadeDuration) / fadeDuration;
            Color fadedColor = tracerColor;
            fadedColor.a = Mathf.Clamp01(1 - progress);
            lineRenderer.startColor = fadedColor;
            lineRenderer.endColor = fadedColor;
        }
    }

    void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }
}