using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    
    public int MaxSlots = 4;
    public List<ItemSlot> Slots;
    public int UsedSlots => Slots.Count;
    public int Coins;

    public Action OnInventoryChange;
    public Action OnCoinChange;

    public bool AddItem(BaseItem item)
    {
        if (Slots == null) Slots = new List<ItemSlot>();

        bool success = false;
        var slot = GetSlot(item);

        if ((slot != null))
        {
            slot.AddOne();

            success = true;
        }
        else if (Slots.Count < MaxSlots)
        {
            slot = new ItemSlot(item);
            Slots.Add(slot);

            success = true;
        }

        OnInventoryChange?.Invoke();

        return success;
    }

    public void RemoveItem(ItemSlot slot)
    {
        if (Slots == null) return;

        if (slot != null)
        {
            slot.RemoveOne();
            if (slot.IsEmpty()) RemoveSlot(slot);
        }

        OnInventoryChange?.Invoke();
    }

    private void RemoveSlot(ItemSlot slot)
    {
        Slots.Remove(slot);
    }

    private ItemSlot GetSlot(BaseItem item)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].CanHold(item)) return Slots[i];
        }

        return null;
    }

    public ItemSlot GetSlot(int i)
    {
        return Slots[i];
    }

    public void ChangeCoins(int amount)
    {
        Coins += amount;
        OnCoinChange?.Invoke();
    }
}