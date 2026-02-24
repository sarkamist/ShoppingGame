// Interface for consumable item, implements Use()
using System;
using System.Collections.Generic;

public interface IConsume
{
    bool Use(BaseConsumableItem item);
}

// Marker attrbiute for non-transferable items
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class NonTransferableItem : Attribute
{

}

public static class InventoryUtilities
{
    private static readonly Dictionary<Type, bool> nonTransferableRegister = new();

    public static bool IsNonTransferable(this BaseItem item)
    {
        if (item == null) return false;

        var t = item.GetType();

        if (nonTransferableRegister.TryGetValue(t, out bool registered))
            return registered;

        bool isNonTransferableItem = Attribute.IsDefined(t, typeof(NonTransferableItem), inherit: true);
        nonTransferableRegister[t] = isNonTransferableItem;
        return isNonTransferableItem;
    }
}