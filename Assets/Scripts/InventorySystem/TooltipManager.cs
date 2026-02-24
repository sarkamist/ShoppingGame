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
        RectTransform canvasRect = (RectTransform)canvas.transform;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            (Vector2)Input.mousePosition + ScreenOffset,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out var localPoint
        );

        rectTransform.anchoredPosition = ClampToCanvas(localPoint, canvasRect);
    }

    private Vector2 ClampToCanvas(Vector2 desired, RectTransform canvasRect)
    {
        Vector2 size = rectTransform.rect.size;

        float minX = canvasRect.rect.xMin + EdgePadding;
        float maxX = canvasRect.rect.xMax - EdgePadding - size.x;

        float maxY = canvasRect.rect.yMax - EdgePadding;
        float minY = canvasRect.rect.yMin + EdgePadding + size.y;

        return new Vector2(
            Mathf.Clamp(desired.x, minX, maxX),
            Mathf.Clamp(desired.y, minY, maxY)
        );
    }
}