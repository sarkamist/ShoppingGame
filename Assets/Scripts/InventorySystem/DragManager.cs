using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance { get; private set; }

    public GameObject GhostImagePrefab;
    [Range(0, 1)] public float DragAlpha = 0.5f; 

    private Canvas canvas;
    private RectTransform ghostRectTransform;
    private Image ghostImage;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        SetupGhostImage();
    }

    private void SetupGhostImage()
    {
        if (ghostImage != null) return;

        GameObject ghostObject = Instantiate(GhostImagePrefab, (RectTransform) transform);
        ghostRectTransform = ghostObject.GetComponent<RectTransform>();
        ghostImage = ghostObject.GetComponent<Image>();

        ghostObject.SetActive(false);
    }

    public void Begin(Sprite sprite, Vector2 screenPos)
    {
        CursorManager.Instance.AddState(CursorState.Drag);

        SetupGhostImage();

        ghostImage.sprite = sprite;
        ghostImage.color = new Color(ghostImage.color.r, ghostImage.color.g, ghostImage.color.b, DragAlpha);

        Move(screenPos);
        ghostRectTransform.gameObject.SetActive(true);
    }

    public void Move(Vector2 screenPos)
    {
        if (ghostRectTransform == null || canvas == null) return;

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) canvas.transform, screenPos, cam, out var localPoint)
        )
        {
            ghostRectTransform.anchoredPosition = localPoint;
        }
    }

    public void Hide()
    {
        CursorManager.Instance.RemoveState(CursorState.Drag);
        if (ghostRectTransform != null) ghostRectTransform.gameObject.SetActive(false);
    }
}