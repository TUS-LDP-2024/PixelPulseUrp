using UnityEngine;

public class UIFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public void FadeToBlack()
    {
        StartCoroutine(FadeCanvasGroup(0f, 1f, 0.7f)); // fade in
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeCanvasGroup(1f, 0f, 0.7f)); // fade out
    }

    private System.Collections.IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
