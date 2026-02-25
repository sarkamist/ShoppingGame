using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/Potion")]
public class Potion : BaseConsumableItem
{
    public override string TypeKey => "item.types.potion";

    public float MaxHealthIncrease;
    public bool BaubleOnConsume;
    public Bauble BaubleItem;

    public override bool Use(IConsume consumer)
    {
        return consumer.Use(this);
    }

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { CurrentHealthIncrease, MaxHealthIncrease };
    }
}