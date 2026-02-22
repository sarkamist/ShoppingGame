using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/Potion")]
public class Potion : BaseConsumableItem
{
    public float MaxHealthIncrease;
    public float CurrentHealthIncrease;

    public override void Use(IConsume consumer)
    {
        consumer.Use(this);
    }

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { CurrentHealthIncrease, MaxHealthIncrease };
    }
}