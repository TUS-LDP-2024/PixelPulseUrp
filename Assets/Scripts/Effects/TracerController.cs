using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TracerController : MonoBehaviour
{
    private ParticleSystem ps;
    private LineRenderer lineRenderer;
    private float duration;
    private float timer;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        // Add LineRenderer for the thin laser effect
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.yellow; // Or your tracer color
        lineRenderer.widthMultiplier = 0.05f; // Very thin line
        lineRenderer.positionCount = 2;
    }

    public void Initialize(Vector3 startPos, Vector3 endPos, float tracerDuration)
    {
        // Set up the line renderer
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Optional: Particle system for impact effect
        var main = ps.main;
        main.startLifetime = tracerDuration;
        main.startSpeed = 0;
        transform.position = endPos;
        ps.Emit(1);

        duration = tracerDuration;
        timer = 0f;

        // Destroy after duration
        Destroy(gameObject, duration);
    }

    void Update()
    {
        // Fade out effect (optional)
        timer += Time.deltaTime;
        float alpha = 1 - (timer / duration);
        Color color = lineRenderer.material.color;
        color.a = alpha;
        lineRenderer.material.color = color;
    }
}