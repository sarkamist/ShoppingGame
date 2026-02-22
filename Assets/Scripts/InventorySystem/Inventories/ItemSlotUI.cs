using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemSlot ItemSlotModel;

    public InventoryUI Inventory;
    public Image Image;
    public Image SelectedOverlay;
    public TextMeshProUGUI AmountText;

    public static Action<PointerEventData, ItemSlotUI> OnSlotMouseClick;

    private int tooltipRequestId;

    public void Bind(ItemSlot slot)
    {
        Image.sprite = slot.Item.Image;
        Image.enabled = true;

        AmountText.text = $"×{slot.Amount.ToString()}";
        AmountText.enabled = (slot.Amount > 1);

        ItemSlotModel = slot;
    }

    public void Unbind()
    {
        Image.sprite = null;
        Image.enabled = false;

        AmountText.enabled = false;

        ItemSlotModel = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ItemSlotModel != null)
        {
            OnSlotMouseClick?.Invoke(eventData, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipManager.Instance == null || ItemSlotModel == null) return;

        BaseItem item = ItemSlotModel.Item;

        string name = LocalizationManager.Instance.Localize(item.NameKey);
        object[] formatArgs = item.GetDescriptionFormatArgs();
        string cost = LocalizationManager.Instance.LocalizeWithFormat("ui.tooltip.cost_line", new object[] { item.Cost });
        string description = LocalizationManager.Instance.LocalizeWithFormat(item.DescriptionKey, formatArgs);
        tooltipRequestId = TooltipManager.Instance.Show(name, $"{cost}\n\n{description}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance == null || ItemSlotModel == null) return;

        TooltipManager.Instance.Hide(tooltipRequestId);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // We need canvas as new UI reference (lazy initialization)
        //if (!canvas) canvas = GetComponentInParent<Canvas>();

        // Store previous reference position
        //parent = transform.parent;

        // Change parent of our item to the canvas
        //transform.SetParent(canvas.transform, true);

        // And set it as last child to be rendered on top of UI
        //transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Moving object around screen using mouse delta
        //transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Find scene objects colliding with mouse point on end dragging
        //RaycastHit2D hitData = Physics2D.GetRayIntersection(
        //    Camera.main.ScreenPointToRay(Input.mousePosition));

        //if (hitData)
        //{
        //    Debug.Log("Drop over object: " + hitData.collider.gameObject.name);
        //
        //    var consumer = hitData.collider.gameObject.GetComponent<IConsume>();
        //
        //    if ((consumer != null) && (item is BaseConsumableItem))
        //    {
        //        (item as BaseConsumableItem).Use(consumer);
        //        inventory.UseItem(item);
        //    }
        //}

        // Changing parent back to slot
        //transform.SetParent(parent.transform);

        // And centering item position
        //transform.localPosition = Vector3.zero;
    }
}