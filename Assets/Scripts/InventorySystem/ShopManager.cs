using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public Player Player;
    public InventoryUI PlayerInventoryUI;
    public InventoryUI ShopInventoryUI;
    public float BuyCostModifier = 1.0f;
    public float SellCostModifier = 0.5f;

    private ItemSlotUI selectedSlotUI;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnPointerClick(PointerEventData eventData, ItemSlotUI itemSlot)
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
        if (selectedSlotUI == null) return;
        selectedSlotUI.SelectedOverlay.enabled = false;
        selectedSlotUI = null;
    }

    public void OnBuyClick()
    {
        if (selectedSlotUI == null) return;

        ManageBuySell(selectedSlotUI, PlayerInventoryUI);
    }

    public void OnSellClick()
    {
        if (selectedSlotUI == null) return;

        ManageBuySell(selectedSlotUI, ShopInventoryUI);
    }

    public void ManageBuySell(ItemSlotUI originSlotUI, InventoryUI targetInventoryUI)
    {
        InventoryUI originInventoryUI = originSlotUI.Inventory;
        BaseItem item = originSlotUI.ItemSlotModel.Item;

        int itemCost = 0;
        if (originInventoryUI == ShopInventoryUI && targetInventoryUI == PlayerInventoryUI)
        {
            itemCost = Mathf.FloorToInt(item.Value * BuyCostModifier);
        }
        else if (originInventoryUI == PlayerInventoryUI && targetInventoryUI == ShopInventoryUI)
        {
            if (item is Junk junk)
            {
                itemCost = Mathf.FloorToInt(item.Value * (junk.IsSoldAtBuyValue ? BuyCostModifier : SellCostModifier));
            }
            else
            {
                itemCost = Mathf.FloorToInt(item.Value * SellCostModifier);
            }
                
        }

        if (itemCost != 0)
        {
            originInventoryUI.InventoryModel.ChangeCoins(itemCost);
            originInventoryUI.InventoryModel.RemoveItem(originSlotUI.ItemSlotModel);
            targetInventoryUI.InventoryModel.ChangeCoins(-itemCost);
            if (item is not Junk)
            {
                targetInventoryUI.InventoryModel.AddItem(item);
            }

            AudioManager.Instance.PlaySFX(AudioManager.Instance.CoinSpent);

            ClearSelectedSlot();
        }
        
    }

    public void OnUseClick()
    {
        if (selectedSlotUI == null) return;

        UseItem(selectedSlotUI, Player);
    }

    public void UseItem(ItemSlotUI originSlotUI, IConsume consumer)
    {
        if (originSlotUI.Inventory == PlayerInventoryUI && originSlotUI.ItemSlotModel.Item is BaseConsumableItem consumable)
        {
            if (consumable.Use(consumer))
            {
                originSlotUI.Inventory.InventoryModel.RemoveItem(originSlotUI.ItemSlotModel);
                ClearSelectedSlot();
            }
        }
    }
}
