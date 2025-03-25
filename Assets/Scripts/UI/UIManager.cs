using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // Canvas image for the health-up flash effect.
    public Image flashImage;
    // Canvas images for the damage flash effect.
    public Image damageFlashImage;
    public Image damageFlashImage2; // Second canvas for damage flash.

    // Set baseline alpha to 0 so the images are hidden by default.
    private float baselineAlpha = 0f;

    private void Awake()
    {
        if (flashImage != null)
        {
            Color color = flashImage.color;
            color.a = baselineAlpha;
            flashImage.color = color;
        }

        if (damageFlashImage != null)
        {
            Color color = damageFlashImage.color;
            color.a = baselineAlpha;
            damageFlashImage.color = color;
        }

        if (damageFlashImage2 != null)
        {
            Color color = damageFlashImage2.color;
            color.a = baselineAlpha;
            damageFlashImage2.color = color;
        }
    }

    // Call this method to trigger the health-up flash effect.
    public void Flash()
    {
        if (flashImage != null)
        {
            StartCoroutine(FlashEffect(flashImage));
        }
    }

    // Call this method to trigger the damage flash effect on both canvases.
    public void DamageFlash()
    {
        if (damageFlashImage != null)
        {
            StartCoroutine(FlashEffect(damageFlashImage));
        }
        if (damageFlashImage2 != null)
        {
            StartCoroutine(FlashEffect(damageFlashImage2));
        }
    }

    // General coroutine to flash a given image.
    private IEnumerator FlashEffect(Image image)
    {
        // Set the flash target alpha to 30/255 (~0.1176).
        Color color = image.color;
        float flashTargetAlpha = 30f / 255f;

        // Immediately set the image's alpha to the flash target.
        color.a = flashTargetAlpha;
        image.color = color;

        float duration = 2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Fade from flashTargetAlpha back to baselineAlpha (0) over duration.
            color.a = Mathf.Lerp(flashTargetAlpha, baselineAlpha, timer / duration);
            image.color = color;
            yield return null;
        }
        // Ensure it ends hidden.
        color.a = baselineAlpha;
        image.color = color;
    }
}
