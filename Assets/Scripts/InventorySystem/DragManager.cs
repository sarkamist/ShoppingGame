using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Range(0, 1)] public float DragAlpha = 0.5f;
    public GameObject dragObject;

    private Image ghostImage;
    private RectTransform ghostRectTransform;
    private RectTransform canvasRectTrasnform;
    private Canvas canvas;
    private Vector2 localPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        canvas = GetComponentInParent<Canvas>();
        ghostImage = dragObject.GetComponent<Image>();
        ghostRectTransform = dragObject.GetComponent<RectTransform>();
        canvasRectTrasnform = canvas.GetComponent<RectTransform>();
        dragObject.SetActive(false);
    }
    public void Begining(Sprite sprite, Vector2 position)
    {
        ghostImage.sprite = sprite;
        ghostImage.color = new Color(ghostImage.color.r, ghostImage.color.g, ghostImage.color.b, DragAlpha);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrasnform, position,
        canvas.worldCamera, out localPoint))
        {
            ghostRectTransform.anchoredPosition = localPoint;
            dragObject.SetActive(true);
        }
    }
    public void Moving(Vector2 position)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), position,
        canvas.worldCamera, out localPoint))
        {
            ghostRectTransform.anchoredPosition = localPoint;
        }
    }
    public void Ending()
    {
        dragObject.SetActive(false);
    }
}