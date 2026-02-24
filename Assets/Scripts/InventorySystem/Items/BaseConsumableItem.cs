public abstract class BaseConsumableItem : BaseItem
{
    public float CurrentHealthIncrease;
    public abstract bool Use(IConsume consumer);

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { CurrentHealthIncrease };
    }
}