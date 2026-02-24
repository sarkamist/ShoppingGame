using UnityEngine;
using UnityEngine.UI;

public enum CursorState { Normal, Drag }

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Sprite NormalSprite;
    public Sprite DragSprite;

    private Canvas canvas;
    private RectTransform rectTransform;
    private Image image;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (rectTransform == null) rectTransform = (RectTransform) transform;
        if (image == null) image = GetComponentInParent<Image>();

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

    public void ChangeState(CursorState state)
    {
        switch (state)
        {
            case CursorState.Normal:
                image.sprite = NormalSprite;
                break;
            case CursorState.Drag:
                image.sprite = DragSprite;
                break;
        }
    }
}
