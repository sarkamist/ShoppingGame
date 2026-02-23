using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/Potion")]
public class Potion : BaseConsumableItem
{
    public float MaxHealthIncrease;
    public bool JunkOnConsume;
    public Junk JunkItem;

    public override bool Use(IConsume consumer)
    {
        return consumer.Use(this);
    }

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { CurrentHealthIncrease, MaxHealthIncrease };
    }
}