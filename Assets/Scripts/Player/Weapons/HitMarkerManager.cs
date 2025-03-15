using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitMarkerManager : MonoBehaviour
{
    public static HitMarkerManager Instance;

    [Header("Hit Marker Settings")]
    // A prefab for the hit marker icon. (Should contain an Image and a CanvasGroup for fading.)
    public GameObject hitMarkerPrefab;
    // The UI Canvas on which the hit markers will be instantiated.
    public Canvas uiCanvas;
    // Sound to play when a hit marker is shown.
    public AudioClip hitSound;
    // Duration over which the hit marker fades away.
    public float fadeDuration = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Establish this instance as the singleton.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Call this method to display a hit marker at the world position where a ray hit an enemy.
    public void ShowHitMarker(Vector3 worldPosition)
    {
        if (uiCanvas == null || hitMarkerPrefab == null)
            return;

        // Convert the world position to screen coordinates.
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        // Instantiate the hit marker prefab as a child of the UI Canvas.
        GameObject hitMarker = Instantiate(hitMarkerPrefab, uiCanvas.transform);
        // Set its position (assuming the Canvas is using Screen Space Overlay or a compatible setup).
        hitMarker.GetComponent<RectTransform>().position = screenPos;

        // Play the hit sound.
        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        // Start a coroutine to fade out and then destroy the hit marker.
        StartCoroutine(FadeAndDestroy(hitMarker));
    }

    private IEnumerator FadeAndDestroy(GameObject marker)
    {
        // Ensure there is a CanvasGroup to control the alpha.
        CanvasGroup canvasGroup = marker.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = marker.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        Destroy(marker);
    }
}
