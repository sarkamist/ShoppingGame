using UnityEngine;

public abstract class BaseItem : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Image;
    public int Cost;
    public bool IsStackable;
    public int StackSize = 1;
}