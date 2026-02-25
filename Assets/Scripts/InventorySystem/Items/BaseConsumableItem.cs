public abstract class BaseConsumableItem : BaseItem
{
    public override string TypeKey => "item.types.consumable";

    public float CurrentHealthIncrease;

    public abstract bool Use(IConsume consumer);

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { CurrentHealthIncrease };
    }
}