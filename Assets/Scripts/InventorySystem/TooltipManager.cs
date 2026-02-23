using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;

    [Header("Positioning")]
    public Vector2 ScreenOffset = new Vector2(12f, -12f);
    public float EdgePadding = 8f;

    private Canvas canvas;
    private RectTransform rectTransform;
    private int currentRequestId;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (rectTransform == null) rectTransform = (RectTransform) transform;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!gameObject.activeSelf) return;
        FollowMouse();
    }

    public int Show(string title, string desc)
    {
        currentRequestId++;

        NameText.text = title;
        NameText.ForceMeshUpdate();
        DescriptionText.text = desc;
        DescriptionText.ForceMeshUpdate();

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        gameObject.SetActive(true);
        FollowMouse();

        return currentRequestId;
    }

    public void Hide(int requestId)
    {
        if (requestId != currentRequestId) return;
        gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        RectTransform canvasRect = (RectTransform) canvas.transform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            (Vector2)Input.mousePosition + ScreenOffset,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = ClampToCanvas(localPoint, canvasRect);
    }

    private Vector2 ClampToCanvas(Vector2 desired, RectTransform canvasRect)
    {
        Vector2 size = rectTransform.rect.size;
        Vector2 min = canvasRect.rect.min + new Vector2(EdgePadding, EdgePadding) + size * 0.5f;
        Vector2 max = canvasRect.rect.max - new Vector2(EdgePadding, EdgePadding) - size * 0.5f;

        return new Vector2(
            Mathf.Clamp(desired.x, min.x, max.x),
            Mathf.Clamp(desired.y, min.y, max.y)
        );
    }
}