using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory System/Items/Food")]
public class Food : BaseConsumableItem
{
    public override bool Use(IConsume consumer)
    {
        return consumer.Use(this);
    }
}