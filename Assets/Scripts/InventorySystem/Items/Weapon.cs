using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory System/Items/Weapon")]
public class Weapon : BaseItem
{
    public float DamageIncrease;

    public override object[] GetDescriptionFormatArgs()
    {
        return new object[] { DamageIncrease };
    }
 }
