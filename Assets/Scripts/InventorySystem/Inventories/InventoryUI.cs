using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryUI : MonoBehaviour
{
    public Inventory InventoryModel;

    [Header("Animation Parameters")]
    public float FlickerDuration = 0.125f;
    public int FlickerCount = 2;

    [Header("References")]
    public TextMeshProUGUI TextCoins;
    public ItemSlotUI SlotPrefab;

    private Image image;
    private List<GameObject> itemSlotList;
    private Color inventoryOriginalColor;
    private Color textCoinsOriginalColor;
    private Coroutine inventoryFlickerRoutine;
    private Coroutine coinFlickerRoutine;

    public Action OnItemSlotPointerClick;

    void Start()
    {
        image = GetComponent<Image>();

        inventoryOriginalColor = image.color;
        textCoinsOriginalColor = TextCoins.color;

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
        var element = Instantiate(SlotPrefab);
        element.transform.SetParent(transform, false);
        element.Inventory = this;

        return element.gameObject;
    }

    public void UpdateCoinUI()
    {
        TextCoins.text = $"{InventoryModel.Coins}";
    }

    public void StartInventoryFlicker(Color flickerColor)
    {
        if (inventoryFlickerRoutine != null) StopCoroutine(inventoryFlickerRoutine);
        inventoryFlickerRoutine = StartCoroutine(InventoryFlickerRoutine(flickerColor));
    }

    private IEnumerator InventoryFlickerRoutine(Color flickerColor)
    {
        for (int i = 0; i < FlickerCount; i++)
        {
            image.color = flickerColor;
            yield return new WaitForSeconds(FlickerDuration);

            image.color = inventoryOriginalColor;
            yield return new WaitForSeconds(FlickerDuration);
        }

        image.color = inventoryOriginalColor;
    }

    public void StartTextCoinsFlicker(Color flickerColor)
    {
        if (coinFlickerRoutine != null) StopCoroutine(coinFlickerRoutine);
        coinFlickerRoutine = StartCoroutine(TextCoinsFlickerRoutine(flickerColor));
    }

    private IEnumerator TextCoinsFlickerRoutine(Color flickerColor)
    {
        for (int i = 0; i < FlickerCount; i++)
        {
            TextCoins.color = flickerColor;
            yield return new WaitForSeconds(FlickerDuration);

            TextCoins.color = textCoinsOriginalColor;
            yield return new WaitForSeconds(FlickerDuration);
        }

        TextCoins.color = textCoinsOriginalColor;
    }
}