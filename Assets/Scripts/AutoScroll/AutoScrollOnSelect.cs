using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Selectable))]
public class AutoScrollOnSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content, _viewport;

    [Header("Scroll Settings")]
    [Tooltip("Smooth scroll speed. Higher = faster.")]
    [SerializeField] private float scrollSpeed = 10f;
    [Tooltip("Viewport margin as fraction (0-0.45). Items within this margin won't trigger scroll.")]
    [Range(0f, 0.45f)]
    [SerializeField] private float scrollMargin = 0.1f;

    private void Awake()
    {
        _scrollRect = GetComponentInParent<ScrollRect>();
        _content = _scrollRect.content;
        _viewport = _scrollRect.viewport ?? _scrollRect.GetComponent<RectTransform>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Canvas.ForceUpdateCanvases();
        RectTransform selectedRT = GetComponent<RectTransform>();

        // Get world corners
        Vector3[] itemCorners = new Vector3[4];
        Vector3[] viewCorners = new Vector3[4];
        Vector3[] contentCorners = new Vector3[4];

        selectedRT.GetWorldCorners(itemCorners);
        _viewport.GetWorldCorners(viewCorners);
        _content.GetWorldCorners(contentCorners);

        // World dimensions
        float worldContentHeight = contentCorners[1].y - contentCorners[0].y;
        float worldViewHeight = viewCorners[1].y - viewCorners[0].y;

        // Calculate limits in world coordinates
        float topLimit = viewCorners[1].y - worldViewHeight * scrollMargin;
        float bottomLimit = viewCorners[0].y + worldViewHeight * scrollMargin;

        float shift = 0f;
        // If item top is above top limit, need scroll up
        if (itemCorners[1].y > topLimit)
        {
            shift = itemCorners[1].y - topLimit;
        }
        // If item bottom is below bottom limit, need scroll down
        else if (itemCorners[0].y < bottomLimit)
        {
            shift = itemCorners[0].y - bottomLimit;
        }
        else
        {
            // Already within viewport with margin
            return;
        }

        // Convert world shift to normalized
        float deltaNorm = shift / (worldContentHeight - worldViewHeight);
        float targetNorm = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition + deltaNorm);

        StartCoroutine(SmoothScrollTo(targetNorm));
    }

    private IEnumerator SmoothScrollTo(float targetNorm)
    {
        float startY = _scrollRect.verticalNormalizedPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * scrollSpeed;
            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startY, targetNorm, t);
            yield return null;
        }
        _scrollRect.verticalNormalizedPosition = targetNorm;
    }
}
