using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory System/Items/Food")]
public class ItemFood : ConsumableItem
{

    public override void Use(IConsume consumer)
    {
        consumer.Use(this);
    }
}

