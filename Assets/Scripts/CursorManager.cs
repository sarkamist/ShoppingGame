using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public enum CursorState { Normal = 0, Attack = 5, Drag = 10 }

    public static CursorManager Instance { get; private set; }

    public Sprite NormalSprite;
    public Sprite DragSprite;
    public Sprite AttackSprite;

    private HashSet<CursorState> activeStates;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Image image;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (rectTransform == null) rectTransform = (RectTransform) transform;
        if (image == null) image = GetComponentInParent<Image>();

        activeStates = new HashSet<CursorState>
        {
            CursorState.Normal
        };

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (!canvas) return;

        switch (GetPrioritaryActiveState())
        {
            case CursorState.Normal:
                image.sprite = NormalSprite;
                break;
            case CursorState.Attack:
                image.sprite = AttackSprite;
                break;
            case CursorState.Drag:
                image.sprite = DragSprite;
                break;
        }

        RectTransform canvasRect = (RectTransform) canvas.transform;

        Camera cam = null;
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                cam,
                out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    public void AddState(CursorState state)
    {
        activeStates.Add(state);
    }

    public void RemoveState(CursorState state)
    {
        activeStates.Remove(state);
    }

    public CursorState GetPrioritaryActiveState()
    {
        return activeStates.Aggregate((max, state) =>
        {
            int maxPriority = Convert.ToInt32(max);
            int currentPriority = Convert.ToInt32(state);

            return (currentPriority > maxPriority) ? state : max;
        });
    }
}
