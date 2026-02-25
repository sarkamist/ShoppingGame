using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    public virtual string TypeKey => "item.types.base";

    public string NameKey;
    public string DescriptionKey;
    public Sprite Image;
    public int Value;
    public bool IsStackable;
    public int StackSize = 1;

    public virtual object[] GetDescriptionFormatArgs() => null;
 }