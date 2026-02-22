using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public PlayerManager Player;
    public InventoryUI PlayerInventoryUI;
    public InventoryUI ShopInventoryUI;
    public float BuyCostModifier = 1.0f;
    public float SellCostModifier = 0.5f;

    private ItemSlotUI selectedSlotUI;

    private void OnEnable()
    {
        ItemSlotUI.OnSlotMouseClick += OnSlotMouseClick;
    }

    private void OnDisable()
    {
        ItemSlotUI.OnSlotMouseClick -= OnSlotMouseClick;
    }

    public void OnSlotMouseClick(PointerEventData eventData, ItemSlotUI itemSlot)
    {
        if (eventData.button == PointerEventData.InputButton.Left && selectedSlotUI != itemSlot)
        {
            ChangeSelectedSlot(itemSlot);
        }
        else if (eventData.button == PointerEventData.InputButton.Right && selectedSlotUI == itemSlot)
        {
            ClearSelectedSlot();
        }
    }

    public void ChangeSelectedSlot(ItemSlotUI itemSlot)
    {
        if (selectedSlotUI != null) selectedSlotUI.SelectedOverlay.enabled = false;
        selectedSlotUI = itemSlot;
        selectedSlotUI.SelectedOverlay.enabled = true;
    }

    public void ClearSelectedSlot()
    {
        selectedSlotUI.SelectedOverlay.enabled = false;
        selectedSlotUI = null;
    }

    public void OnBuyClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != ShopInventoryUI) return;

        BaseItem item = selectedSlotUI.ItemSlotModel.Item;
        int itemCost = Mathf.FloorToInt(item.Cost * BuyCostModifier);

        Debug.Log("Buying...");
        if (PlayerInventoryUI.InventoryModel.Coins >= itemCost)
        {
            ShopInventoryUI.InventoryModel.ChangeCoins(itemCost);
            ShopInventoryUI.InventoryModel.RemoveItem(selectedSlotUI.ItemSlotModel);
            PlayerInventoryUI.InventoryModel.ChangeCoins(-itemCost);
            PlayerInventoryUI.InventoryModel.AddItem(item);

            ClearSelectedSlot();
        }
        else
        {
            Debug.Log($"Not enough Player coins to buy {item.NameKey}!");
        }
    }

    public void OnSellClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != PlayerInventoryUI) return;

        BaseItem item = selectedSlotUI.ItemSlotModel.Item;
        int itemCost = Mathf.FloorToInt(item.Cost * SellCostModifier);

        Debug.Log("Selling...");
        if (ShopInventoryUI.InventoryModel.Coins >= itemCost)
        {
            ShopInventoryUI.InventoryModel.ChangeCoins(-itemCost);
            ShopInventoryUI.InventoryModel.AddItem(item);
            PlayerInventoryUI.InventoryModel.ChangeCoins(+itemCost);
            PlayerInventoryUI.InventoryModel.RemoveItem(selectedSlotUI.ItemSlotModel);

            ClearSelectedSlot();
        }
        else
        {
            Debug.Log($"Not enough Shop coins to sell {item.NameKey}!");
        }
    }

    public void OnUseClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != PlayerInventoryUI) return;

        if (selectedSlotUI.ItemSlotModel.Item is BaseConsumableItem consumable)
        {
            PlayerInventoryUI.InventoryModel.RemoveItem(selectedSlotUI.ItemSlotModel);
            consumable.Use(Player);

            ClearSelectedSlot();
        }
    }
}
