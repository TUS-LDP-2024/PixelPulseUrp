using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Image flashImage;
    public Image damageFlashImage;
    public Image damageFlashImage2;

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

    public void Flash()
    {
        if (flashImage != null)
        {
            StartCoroutine(FlashEffect(flashImage));
        }
    }

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

    private IEnumerator FlashEffect(Image image)
    {
        Color color = image.color;
        float flashTargetAlpha = 30f / 255f;

        color.a = flashTargetAlpha;
        image.color = color;

        float duration = 2f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(flashTargetAlpha, baselineAlpha, timer / duration);
            image.color = color;
            yield return null;
        }
        color.a = baselineAlpha;
        image.color = color;
    }

    // HUD FADE-IN
    public void FadeInHUD()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 1f, 1f));
        }
        else
        {
            Debug.LogWarning("UIManager: No CanvasGroup found for HUD fade.");
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}
