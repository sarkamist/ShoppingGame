using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    public string NameKey;
    public string DescriptionKey;
    public Sprite Image;
    public int Cost;
    public bool IsStackable;
    public int StackSize = 1;

    public virtual object[] GetDescriptionFormatArgs() => null;
 }