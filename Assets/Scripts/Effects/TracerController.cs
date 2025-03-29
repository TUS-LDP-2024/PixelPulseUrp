using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TracerController : MonoBehaviour
{
    private ParticleSystem ps;
    private LineRenderer lineRenderer;
    private float duration;
    private float timer;
    private Material lineMaterial;

    [Header("Tracer Settings")]
    public Color tracerColor = Color.yellow; // Set your desired color in Inspector
    public float lineWidth = 0.05f;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        // Get or add LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Create material using a built-in shader that supports color
        lineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineMaterial.color = tracerColor;
        lineRenderer.material = lineMaterial;

        // Configure line appearance
        lineRenderer.startColor = tracerColor;
        lineRenderer.endColor = tracerColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    public void Initialize(Vector3 startPos, Vector3 endPos, float tracerDuration)
    {
        // Set line positions
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Configure particle system for impact effect
        var main = ps.main;
        main.startLifetime = tracerDuration;
        main.startSpeed = 0;
        transform.position = endPos;
        ps.Play();

        duration = tracerDuration;
        timer = 0f;

        Destroy(gameObject, duration);
    }

    void Update()
    {
        // Fade out effect
        timer += Time.deltaTime;
        float alpha = Mathf.Clamp01(1 - (timer / duration));

        // Update both material and line renderer colors
        Color fadingColor = tracerColor;
        fadingColor.a = alpha;

        lineMaterial.color = fadingColor;
        lineRenderer.startColor = fadingColor;
        lineRenderer.endColor = fadingColor;
    }

    void OnDestroy()
    {
        // Clean up the material to prevent memory leaks
        if (Application.isPlaying && lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }
}