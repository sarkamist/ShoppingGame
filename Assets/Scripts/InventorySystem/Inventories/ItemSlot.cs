using System;

[Serializable]
public class ItemSlot
{
    public BaseItem Item;
    public int Amount;

    public ItemSlot(BaseItem item)
    {
        this.Item = item;
        Amount = 1;
    }

    internal bool CanHold(BaseItem item)
    {
        if (item.IsStackable && item == Item) return (Amount < item.StackSize);

        return false;
    }

    internal void AddOne()
    {
        Amount++;
    }

    internal void RemoveOne()
    {
        Amount--;
    }

    public bool IsEmpty()
    {
        return (Amount < 1);
    }
}