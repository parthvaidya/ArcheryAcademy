using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleScrollNavigator : MonoBehaviour
{
    [Header("References (drag from Inspector)")]
    public ScrollRect scrollRect;            // Scroll View component
    public RectTransform content;            // Content RectTransform (optional - auto assigned)

    [Header("Navigation")]
    public int startPage = 0;                // which page to start on (0-based)
    [Tooltip("If > 0, pages will smoothly snap over this duration (seconds). If 0 => instant snap.")]
    public float snapDuration = 0.25f;

    int pageCount = 0;
    int currentPage = 0;
    float[] pagePositions;                   // normalized positions (0..1)

    void Start()
    {
        // auto-assign if not set
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogError("SimpleScrollNavigator: No ScrollRect assigned or found on the GameObject.");
            enabled = false;
            return;
        }

        if (content == null) content = scrollRect.content;
        RebuildPages();
        SetPage(startPage, true);
    }

    // Call this if you change the children at runtime
    public void RebuildPages()
    {
        if (content == null) return;
        pageCount = Mathf.Max(1, content.childCount);
        pagePositions = new float[pageCount];

        if (pageCount == 1)
        {
            pagePositions[0] = 0f;
        }
        else
        {
            for (int i = 0; i < pageCount; i++)
                pagePositions[i] = (float)i / (pageCount - 1); // 0..1 evenly spaced
        }

        currentPage = Mathf.Clamp(startPage, 0, pageCount - 1);
    }

    // Hook these to your Left/Right buttons OnClick()
    public void Next()
    {
        if (pageCount <= 1) return;
        SetPage(Mathf.Min(currentPage + 1, pageCount - 1));
    }

    public void Previous()
    {
        if (pageCount <= 1) return;
        SetPage(Mathf.Max(currentPage - 1, 0));
    }

    /// <summary>
    /// Move to a page (0-based). If instant = true, set immediately.
    /// </summary>
    public void SetPage(int pageIndex, bool instant = false)
    {
        if (pagePositions == null || pagePositions.Length == 0) RebuildPages();

        pageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
        currentPage = pageIndex;
        float target = pagePositions[currentPage];

        if (snapDuration <= 0f || instant)
        {
            scrollRect.horizontalNormalizedPosition = target;
            StopAllCoroutines();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(SmoothSnap(scrollRect.horizontalNormalizedPosition, target, snapDuration));
        }
    }

    IEnumerator SmoothSnap(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.SmoothStep(0f, 1f, t / duration); // smooth easing
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(from, to, lerp);
            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = to;
    }

#if UNITY_EDITOR
    // helpful in editor if you change children while editing
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
            if (scrollRect != null && content == null) content = scrollRect.content;
        }
    }
#endif
}