using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform rectTransform;

    void Awake()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!rectTransform) rectTransform = (RectTransform) transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (!canvas) return;

        RectTransform canvasRect = (RectTransform) canvas.transform;

        Camera cam = null;
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                cam,
                out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
