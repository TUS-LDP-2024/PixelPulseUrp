using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Image flashImage;
    // Set baseline alpha to 0 so the image is hidden by default.
    private float baselineAlpha = 0f;

    private void Awake()
    {
        if (flashImage != null)
        {
            Color color = flashImage.color;
            color.a = baselineAlpha;
            flashImage.color = color;
        }
    }

    // Call this method to trigger the flash effect.
    public void Flash()
    {
        if (flashImage != null)
        {
            StartCoroutine(FlashEffect());
        }
    }

    private IEnumerator FlashEffect()
    {
        // Set the flash target alpha to 30/255 (~0.1176).
        Color color = flashImage.color;
        float flashTargetAlpha = 30f / 255f;

        // Immediately set the image's alpha to the flash target.
        color.a = flashTargetAlpha;
        flashImage.color = color;

        float duration = 2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Fade from flashTargetAlpha back to baselineAlpha (0) over duration.
            color.a = Mathf.Lerp(flashTargetAlpha, baselineAlpha, timer / duration);
            flashImage.color = color;
            yield return null;
        }
        // Ensure it ends hidden.
        color.a = baselineAlpha;
        flashImage.color = color;
    }
}
