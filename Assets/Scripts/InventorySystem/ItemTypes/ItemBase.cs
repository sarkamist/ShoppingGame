using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string Name;
    public Sprite ImageUI;
    public string Description;
    public int Cost;
    public bool IsStackable;
}