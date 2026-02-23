using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory InventoryModel;

    public TextMeshProUGUI CoinText;
    public ItemSlotUI SlotPrefab;

    List<GameObject> itemSlotList;

    public Action OnItemSlotPointerClick;

    void Start()
    {
        FillInventoryUI(InventoryModel);
        UpdateCoinUI();
    }

    private void OnEnable()
    {
        InventoryModel.OnInventoryChange += UpdateInventoryUI;
        InventoryModel.OnCoinChange += UpdateCoinUI;
    }

    private void OnDisable()
    {
        InventoryModel.OnInventoryChange -= UpdateInventoryUI;
        InventoryModel.OnCoinChange -= UpdateCoinUI;
    }

    private void UpdateInventoryUI()
    {
        FillInventoryUI(InventoryModel);
    }

    private void FillInventoryUI(Inventory inventory)
    {
        if (itemSlotList == null)
        {
            itemSlotList = new List<GameObject>();
            for (int i = 0; i < inventory.MaxSlots; i++)
            {
                itemSlotList.Add(CreateUISlot());
            }
        }

        if (inventory.UsedSlots < 0) return;

        int j = 0;
        while (j < inventory.UsedSlots)
        {
            itemSlotList[j].GetComponent<ItemSlotUI>().Bind(inventory.GetSlot(j));
            j++;
        }
        while (j < inventory.MaxSlots)
        {
            itemSlotList[j].GetComponent<ItemSlotUI>().Unbind();
            j++;
        }
    }

    private GameObject CreateUISlot()
    {
        var element = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
        element.Inventory = this;

        return element.gameObject;
    }

    public void UpdateCoinUI()
    {
        CoinText.text = $"{InventoryModel.Coins}";
    }
}