using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class AnimationUtil
{
    // 淡入动画
    public static IEnumerator FadeIn(GameObject target, float duration, System.Action onComplete = null)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(target);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1;
        onComplete?.Invoke();
    }

    // 淡出动画
    public static IEnumerator FadeOut(GameObject target, float duration, System.Action onComplete = null)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(target);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        onComplete?.Invoke();
    }

    // 缩放进入动画
    public static IEnumerator ScaleIn(GameObject target, float duration, System.Action onComplete = null)
    {
        Transform transform = target.transform;
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = originalScale;
        onComplete?.Invoke();
    }

    // 缩放退出动画
    public static IEnumerator ScaleOut(GameObject target, float duration, System.Action onComplete = null)
    {
        Transform transform = target.transform;
        Vector3 originalScale = transform.localScale;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        transform.localScale = originalScale;
        onComplete?.Invoke();
    }

    // 滑入动画
    public static IEnumerator SlideIn(GameObject target, float duration, System.Action onComplete = null)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector3(originalPos.x + Screen.width, originalPos.y, originalPos.z);

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector3.Lerp(
                new Vector3(originalPos.x + Screen.width, originalPos.y, originalPos.z),
                originalPos,
                elapsed / duration
            );
            yield return null;
        }

        rect.anchoredPosition = originalPos;
        onComplete?.Invoke();
    }

    // 滑出动画
    public static IEnumerator SlideOut(GameObject target, float duration, System.Action onComplete = null)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector3.Lerp(
                originalPos,
                new Vector3(originalPos.x + Screen.width, originalPos.y, originalPos.z),
                elapsed / duration
            );
            yield return null;
        }

        rect.anchoredPosition = originalPos;
        onComplete?.Invoke();
    }

    // 获取或添加CanvasGroup组件
    private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }
}
