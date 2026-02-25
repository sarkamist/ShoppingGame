using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Serializable]
    public struct ItemRollWeight
    {
        [Min(1)] public int weight;
        public BaseItem item;
    }

    public static ShopManager Instance { get; private set; }

    [Header("Shop Parameters")]
    public List<ItemRollWeight> PlayerRefillableItems;
    public List<ItemRollWeight> ShopRefillableItems;
    public float BuyCostModifier = 1.0f;
    public float SellCostModifier = 0.5f;
    public Color ErrorFlickerColor = Color.darkRed;

    [Header("Tooltip Parameters")]
    public string MerchantNameKey;
    public string MerchantDescriptionKey;
    public string BuyDescriptionKey;
    public string SellDescriptionKey;
    public string UseDescriptionKey;
    public string LeaveShopDescriptionKey;
    public string SearchLootDescriptionKey;
    public string RefillShopDescriptionKey;
    
    [Header("References")]
    public Player Player;
    public InventoryUI PlayerInventoryUI;
    public InventoryUI ShopInventoryUI;
    public GameObject GoldParticlesPrefab;
    public Button BuyButton;
    public Button SellButton;
    public Button UseButton;

    private ItemSlotUI selectedSlotUI;
    private int tooltipRequestId;

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
        BuyButton.interactable = (
            selectedSlotUI != null
            && selectedSlotUI.Inventory == ShopInventoryUI
        );
        SellButton.interactable = (
            selectedSlotUI != null
            && selectedSlotUI.Inventory == PlayerInventoryUI
        );
        UseButton.interactable = (
            selectedSlotUI != null
            && selectedSlotUI.Inventory == PlayerInventoryUI
        );
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

    public void OnBuyPointerEnter()
    {
        ShowTooltip(
            null,
            BuyDescriptionKey,
            new object[] {
                BuyCostModifier
            }
        );
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

    public void OnSellPointerEnter()
    {
        ShowTooltip(
            null,
            SellDescriptionKey,
            new object[] {
                SellCostModifier
            }
        );
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
            if (item is Bauble junk)
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
                targetInventoryUI.StartTextCoinsFlicker(ErrorFlickerColor);
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

    public void OnUsePointerEnter()
    {
        ShowTooltip(null, UseDescriptionKey);
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

    public void OnLeaveShopClick()
    {
        SceneChanger.Instance.OnVictory();
    }

    public void OnLeaveShopPointerEnter()
    {
        ShowTooltip(null, LeaveShopDescriptionKey);
    }

    public void OnWitchPointerEnter()
    {
        ShowTooltip(
            MerchantNameKey,
            MerchantDescriptionKey,
            new object[] {
                BuyCostModifier,
                SellCostModifier
            }
        );
    }

    public void OnSearchLootClick()
    {
        BaseItem newItem = GetRandomRefillableItem(PlayerRefillableItems);

        if (!PlayerInventoryUI.InventoryModel.CanHold(newItem))
        {
            PlayerInventoryUI.StartInventoryFlicker(ErrorFlickerColor);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.PopSound);
        PlayerInventoryUI.InventoryModel.AddItem(newItem);
    }

    public void OnSearchLootPointerEnter()
    {
        ShowTooltip(null, SearchLootDescriptionKey);
    }

    public void OnRefillShopClick()
    {
        BaseItem newItem = GetRandomRefillableItem(ShopRefillableItems);

        if (!ShopInventoryUI.InventoryModel.CanHold(newItem))
        {
            ShopInventoryUI.StartInventoryFlicker(ErrorFlickerColor);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.Error);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.PopSound);
        ShopInventoryUI.InventoryModel.AddItem(newItem);
    }

    public void OnRefillShopPointerEnter()
    {
        ShowTooltip(null, RefillShopDescriptionKey);
    }

    private BaseItem GetRandomRefillableItem(List<ItemRollWeight> list)
    {
        int totalWeight = GetTotalRefillabeItemsWeight(list);
        int rdm = UnityEngine.Random.Range(0, totalWeight);

        foreach (ItemRollWeight entry in list)
        {
            if (rdm < entry.weight)
            {
                return entry.item;
            }

            rdm -= entry.weight;
        }

        return default;
    }

    private int GetTotalRefillabeItemsWeight(List<ItemRollWeight> list)
    {
        return list.Sum((entry) => entry.weight);
    }

    public void ShowTooltip(string nameKey, string descriptionKey, object[] formatArgs = null)
    {
        if (TooltipManager.Instance == null) return;

        string name = LocalizationManager.Instance.Localize(nameKey);

        string description = formatArgs != null
        ? LocalizationManager.Instance.LocalizeWithFormat(descriptionKey, formatArgs)
        : LocalizationManager.Instance.Localize(descriptionKey);

        tooltipRequestId = TooltipManager.Instance.Show(name, description);
    }

    public void OnPointerExit()
    {
        if (TooltipManager.Instance == null) return;

        TooltipManager.Instance.Hide(tooltipRequestId);
    }
}
