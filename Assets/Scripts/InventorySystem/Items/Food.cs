using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory System/Items/Food")]
public class Food : BaseConsumableItem
{
    public override string TypeKey => "item.types.food";

    public override bool Use(IConsume consumer)
    {
        return consumer.Use(this);
    }
}