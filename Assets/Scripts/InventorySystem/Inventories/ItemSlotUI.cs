using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    private Canvas canvas;
    private Transform dragParent;
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
        if (ShopManager.Instance == null || ItemSlotModel == null) return;

        ShopManager.Instance.OnPointerClick(eventData, this);
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
        if (ItemSlotModel == null) return;
        if (ShopManager.Instance != null) ShopManager.Instance.ClearSelectedSlot();
        CursorManager.Instance.ChangeState(CursorState.Drag);

        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, DragManager.Instance.DragAlpha);
        DragManager.Instance.Begin(ItemSlotModel.Item.Image, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.Instance.Move(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Image.color = Color.white;
        DragManager.Instance.Hide();
        CursorManager.Instance.ChangeState(CursorState.Normal);

        if (ItemSlotModel == null) return;

        var go = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log("checking hit");
        if (go == null) return;

        Debug.Log(go);

        if (go.GetComponent<Player>() is Player player)
        {
            ShopManager.Instance.UseItem(this, player);
        }
        else if (go.GetComponent<ItemSlotUI>() is ItemSlotUI itemSlotUI)
        {
            if (Inventory == itemSlotUI.Inventory) return;

            ShopManager.Instance.ManageBuySell(this, itemSlotUI.Inventory);
        }
        else if (go.GetComponent<InventoryUI>() is InventoryUI inventory)
        {
            if (Inventory == inventory) return;

            ShopManager.Instance.ManageBuySell(this, inventory);
        }
    }
}