using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Parameters")]
    public float BuyCostModifier = 1.0f;
    public float SellCostModifier = 0.5f;
    public Color ErrorFlickerColor = Color.darkRed;

    [Header("References")]
    public Player Player;
    public InventoryUI PlayerInventoryUI;
    public InventoryUI ShopInventoryUI;
    public GameObject GoldParticlesPrefab;
    public Button BuyButton;
    public Button SellButton;
    public Button UseButton;

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

        UpdateShopButtons();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.ItemSelect);
    }

    public void ClearSelectedSlot()
    {
        if (selectedSlotUI == null) return;
        selectedSlotUI.SelectedOverlay.enabled = false;
        selectedSlotUI = null;

        UpdateShopButtons();
    }

    public void UpdateShopButtons()
    {
        BuyButton.interactable = (selectedSlotUI != null && selectedSlotUI.Inventory == ShopInventoryUI);
        SellButton.interactable = (selectedSlotUI != null && selectedSlotUI.Inventory == PlayerInventoryUI);
        UseButton.interactable = (selectedSlotUI != null && selectedSlotUI.Inventory == PlayerInventoryUI);
    }

    public void OnBuyClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != ShopInventoryUI)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

        ManageBuySell(selectedSlotUI, PlayerInventoryUI);
    }

    public void OnSellClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != PlayerInventoryUI)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

        ManageBuySell(selectedSlotUI, ShopInventoryUI);
    }

    public void ManageBuySell(ItemSlotUI originSlotUI, InventoryUI targetInventoryUI)
    {
        InventoryUI originInventoryUI = originSlotUI.Inventory;
        Inventory targetInventory = targetInventoryUI.InventoryModel;
        Inventory originInventory = originInventoryUI.InventoryModel;
        BaseItem item = originSlotUI.ItemSlotModel.Item;

        if (!targetInventory.CanHold(item) && !item.IsNonTransferable())
        {
            targetInventoryUI.StartInventoryFlicker(ErrorFlickerColor);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

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

        if (itemCost >= 0)
        {
            if (targetInventory.Coins >= itemCost)
            {
                originInventory.ChangeCoins(itemCost);
                targetInventory.ChangeCoins(-itemCost);

                Instantiate(
                    GoldParticlesPrefab,
                    targetInventoryUI.TextCoins.rectTransform.position,
                    Quaternion.identity,
                    targetInventoryUI.transform
                ).transform.SetAsLastSibling();

                AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.CoinSpent);
            }
            else
            {
                targetInventoryUI.StartCoinTextFlicker(ErrorFlickerColor);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);

                return;
            } 
        }

        originInventory.RemoveItem(originSlotUI.ItemSlotModel);
        if (!item.IsNonTransferable())
        {
            targetInventory.AddItem(item);
        }

        ClearSelectedSlot();
    }

    public void OnUseClick()
    {
        if (selectedSlotUI == null || selectedSlotUI.Inventory != PlayerInventoryUI)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

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

    public void OnLeaveClick()
    {
        SceneChanger.Instance.OnVictory();
    }
}
