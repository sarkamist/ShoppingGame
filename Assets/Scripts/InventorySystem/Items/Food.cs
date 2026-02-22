using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory System/Items/Food")]
public class Food : BaseConsumableItem
{
    public float CurrentHealthIncrease;

    public override void Use(IConsume consumer)
    {
        consumer.Use(this);
    }
}